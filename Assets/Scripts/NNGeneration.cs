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
            tex = ApplyFastBlur(ApplyFastBlur(tex));
        }

        if (applyGaussianBlur)
        {
            tex = ApplyGaussianBlur(ApplyGaussianBlur(tex));
        }

        inputTensor.Dispose();
        output.Dispose();
        return tex;
    }

    Texture2D ApplyFastBlur(Texture2D inputTexture, int radius = 5)
    {
        int width = inputTexture.width;
        int height = inputTexture.height;
        Color[] pixels = inputTexture.GetPixels();
        Color[] blurredPixels = new Color[pixels.Length];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                Color sum = Color.clear;
                int count = 0;

                for (int dy = -radius; dy <= radius; dy++) {
                    for (int dx = -radius; dx <= radius; dx++) {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < width && ny >= 0 && ny < height) {
                            int index = ny * width + nx;
                            sum += pixels[index];
                            count++;
                        }
                    }
                }

                int currentIndex = y * width + x;
                blurredPixels[currentIndex] = sum / count;
            }
        }

        Texture2D outputTexture = new Texture2D(width, height);
        outputTexture.SetPixels(blurredPixels);
        outputTexture.Apply();
        return outputTexture;
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