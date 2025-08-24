using UnityEngine;
using Unity.Barracuda;
using System.IO;
using UnityEngine.UI;

public class NNGeneration : MonoBehaviour
{
    public NNModel modelAsset;
    public bool applyBlur;
    public bool applyGaussianBlur;
    public int latentSize = 512;
    public int outputSize = 512;

    private Model runtimeModel;
    private IWorker worker;
    public System.Action OnGenerate;

    void Start()
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        if (worker == null)
            worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
    }
    public void Generate(TerrainType[] _regions)
    {
        OnGenerate?.Invoke();
        var heightMap = GenerateHeightmap();
        FindObjectOfType<GenerateFromImage>().Generate(_regions, heightMap);
    }

    private Texture2D GenerateHeightmap()
    {
        float[] latentVector = new float[latentSize];
        for (int i = 0; i < latentSize; i++)
        {
            latentVector[i] = Random.Range(-10f, 10f);
        }

        Tensor inputTensor = new Tensor(1, latentSize, latentVector);

        worker.Execute(inputTensor);
        Tensor output = worker.PeekOutput();
        float[] data = output.ToReadOnlyArray();

        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGBA32, false);
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                float value = data[y * 512 + x];
                value = Mathf.Pow(value, 2f);
                tex.SetPixel(x, y, new Color(value, value, value));
            }
        }
        tex.Apply();

        if (applyBlur)
        {
            tex = ApplyFastBlur(tex);//ApplyFastBlur(ApplyFastBlur(tex));
        }

        if (applyGaussianBlur)
        {
            tex = ApplyGaussianBlur(ApplyGaussianBlur(tex));
        }

        inputTensor.Dispose();
        output.Dispose();
        return tex;
    }

    Texture2D ApplyFastBlur(Texture2D input, int radius = 5)
    {
    int w = input.width;
    int h = input.height;
    Color[] pixels = input.GetPixels();
    Color[] temp = new Color[pixels.Length];
    Color[] output = new Color[pixels.Length];

    int kernelSize = radius * 2 + 1;

    for (int y = 0; y < h; y++)
    {
        Color sum = Color.clear;
        int index = y * w;

        for (int x = -radius; x <= radius; x++)
        {
            int clampedX = Mathf.Clamp(x, 0, w - 1);
            sum += pixels[index + clampedX];
        }

        temp[index] = sum / kernelSize;

        for (int x = 1; x < w; x++)
        {
            int removeX = Mathf.Clamp(x - radius - 1, 0, w - 1);
            int addX = Mathf.Clamp(x + radius, 0, w - 1);

            sum += pixels[index + addX];
            sum -= pixels[index + removeX];

            temp[index + x] = sum / kernelSize;
        }
    }

    for (int x = 0; x < w; x++)
    {
        Color sum = Color.clear;
        for (int y = -radius; y <= radius; y++)
        {
            int clampedY = Mathf.Clamp(y, 0, h - 1);
            sum += temp[clampedY * w + x];
        }

        output[x] = sum / kernelSize;

        for (int y = 1; y < h; y++)
        {
            int removeY = Mathf.Clamp(y - radius - 1, 0, h - 1);
            int addY = Mathf.Clamp(y + radius, 0, h - 1);

            sum += temp[addY * w + x];
            sum -= temp[removeY * w + x];

            output[y * w + x] = sum / kernelSize;
        }
    }

    Texture2D blurred = new Texture2D(w, h);
    blurred.SetPixels(output);
    blurred.Apply();
    return blurred;
}

    
    private Texture2D ApplyGaussianBlur(Texture2D source)
    {
        Texture2D blurred = new Texture2D(source.width, source.height);

        float[,] kernel = new float[,]
        {
            { 1f, 4f, 7f, 4f, 1f },
            { 4f, 16f, 26f, 16f, 4f },
            { 7f, 26f, 41f, 26f, 7f },
            { 4f, 16f, 26f, 16f, 4f },
            { 1f, 4f, 7f, 4f, 1f }
        };

        float kernelSum = 273f;

        int kernelRadius = 2;

        int width = source.width;
        int height = source.height;

        Color[] srcPixels = source.GetPixels();
        Color[] dstPixels = new Color[srcPixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color c = Color.black;

                for (int ky = -kernelRadius; ky <= kernelRadius; ky++)
                {
                    int ny = Mathf.Clamp(y + ky, 0, height - 1);

                    for (int kx = -kernelRadius; kx <= kernelRadius; kx++)
                    {
                        int nx = Mathf.Clamp(x + kx, 0, width - 1);

                        float weight = kernel[ky + kernelRadius, kx + kernelRadius];
                        Color sample = srcPixels[ny * width + nx];
                        c += sample * weight;
                    }
                }

                c /= kernelSum;
                dstPixels[y * width + x] = c;
            }
        }

        blurred.SetPixels(dstPixels);
        blurred.Apply();
        return blurred;
    }



    void OnDestroy()
    {
        worker?.Dispose();
    }
}