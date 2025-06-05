using UnityEngine;
using Unity.Barracuda;
using System.IO;
using UnityEngine.UI;

public class NNGeneration : MonoBehaviour
{
    public NNModel modelAsset; // Przypisz w inspectorze
    public int latentSize = 512; // Taki jak w twoim modelu
    public int outputSize = 512; // 256 lub 512, zależnie od wyjścia ONNX

    private Model runtimeModel;
    private IWorker worker;

    public void Generate(TerrainType[] _regions)
    {
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);
        var heightMap = GenerateHeightmap();
        FindObjectOfType<GenerateFromImage>().Generate(_regions, heightMap);
    }

    private Texture2D GenerateHeightmap()
    {
        float[] latentVector = new float[latentSize];
        for (int i = 0; i < latentSize; i++)
        {
            latentVector[i] = Random.Range(-3f, 3f);
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
                tex.SetPixel(x, y, new Color(value, value, value));
            }
        }
        tex.Apply();
        Debug.Log("Heightmap wygenerowana");
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/heightmap_output.png", bytes);
        inputTensor.Dispose();
        output.Dispose();
        return tex;
    }
    

    void OnDestroy()
    {
        worker?.Dispose();
    }
}