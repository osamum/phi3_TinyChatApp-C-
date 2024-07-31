// SLM Phi-3 モデルとコマンドラインでチャットするサンプル
using Microsoft.ML.OnnxRuntimeGenAI;

//phi-3 モデルへのパス (※デバッグ実行の際は相対パスでなく必ずフルパスを指定)
string modelPath = @".\onnx-models\phi3\mini\";
//string modelPath = @".\onnx-models\phi3\mini\";

Console.WriteLine("-------------");
Console.WriteLine("Hello, Phi!");
Console.WriteLine("-------------");
Console.WriteLine("Model path: " + modelPath);

using Model model = new Model(modelPath);
using Tokenizer tokenizer = new Tokenizer(model);

do
{
    Console.WriteLine("Prompt:");
    string prompt = Console.ReadLine() ?? string.Empty;
    var sequences = tokenizer.Encode($"<|user|>{prompt}<|end|><|assistant|>");
    using GeneratorParams generatorParams = new GeneratorParams(model);
    generatorParams.SetSearchOption("max_length", 200);
    generatorParams.SetInputSequences(sequences);

    /*--- Stream を使用して一文字ずづ出力 ---*/
    using var tokenizerStream = tokenizer.CreateStream();
    using var generator = new Generator(model, generatorParams);
    while (!generator.IsDone())
    {
        generator.ComputeLogits();
        generator.GenerateNextToken();
        Console.Write(tokenizerStream.Decode(generator.GetSequence(0)[^1]));
    }
    Console.WriteLine();
    /*--- ココマデ ---*/

    /* --- Stream を使用せずまとめて出力する場合 ---
    var outputSequences = model.Generate(generatorParams);
    var outputString = tokenizer.Decode(outputSequences[0]);
    Console.WriteLine(outputString);
    */

} while (true);