#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace ReplaceBenchmark;

internal class Program
{
    static void Main(string[] args)
    {
        var fi = new FileInfo(@"rrt.osb");
        //var fi = new FileInfo(@"light.osb");
        //var fi = new FileInfo(@"test.osb");
        if (!fi.Exists)
            throw new FileNotFoundException("Test file does not exists: " + fi.FullName);
        Environment.SetEnvironmentVariable("test_osb_path", fi.FullName);

        var summary = BenchmarkRunner.Run<ReplacingTask>(/*config*/);
    }
}

[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net48)]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MarkdownExporter]
public class ReplacingTask
{
    private readonly string _path;
    private readonly List<KeyValuePair<string, string>> _dic;
    public ReplacingTask()
    {
        var path = Environment.GetEnvironmentVariable("test_osb_path");
        _path = path;
        Console.WriteLine(_path);
        _dic = new Dictionary<string, string>()
        {
            ["$1"] = "BottomLeft",
            ["$2"] = "BottomCentre",
            ["$3"] = "BottomRight",
            ["$4"] = "CentreLeft",
            ["$5"] = "Centre",
            ["$6"] = "CentreRight",
            ["$7"] = "TopLeft",
            ["$8"] = "TopCentre",
            ["$9"] = "TopRight",
            ["$a"] = "Sprite",
            ["$b"] = "Animation",
            ["$c"] = "Background",
            ["$d"] = "Foreground",
            ["$e"] = "Fail",
            ["$f"] = "Pass",
            ["$z"] = "Sprite,$d,$5,",
            ["$y"] = "Sprite,$d,$7,",
            ["$x"] = "Sprite,$c,$5,",
            ["$w"] = "Sprite,$c,$7,",
            ["$v"] = ",320,240",
            ["$Ga"] = " F,0,",
            ["$Gb"] = " F,1,",
            ["$Gc"] = " F,2,",
            ["$Gd"] = " M,0,",
            ["$Ge"] = " M,1,",
            ["$Gf"] = " M,2,",
            ["$Gg"] = " MX,0,",
            ["$Gh"] = " MX,1,",
            ["$Gi"] = " MX,2,",
            ["$Gj"] = " MY,0,",
            ["$Gk"] = " MY,1,",
            ["$Gl"] = " MY,2,",
            ["$Gm"] = " P,0,",
            ["$Gn"] = " P,1,",
            ["$Go"] = " P,2,",
            ["$Gp"] = " R,0,",
            ["$Gq"] = " R,1,",
            ["$Gr"] = " R,2,",
            ["$Gs"] = " S,0,",
            ["$Gt"] = " S,1,",
            ["$Gu"] = " S,2,",
            ["$Gv"] = " V,0,",
            ["$Gw"] = " V,1,",
            ["$Gx"] = " V,2,",
            ["$Ha"] = " C,0,",
            ["$Hb"] = " C,1,",
            ["$Hc"] = " C,2,",
            ["$i"] = "0,1",
            ["$j"] = "1,0",
            ["$R000"] = "\"sb\\element\\square.png\"",
            ["$R001"] = "\"sb\\element\\realnoise/noise_.png\"",
            ["$R002"] = "\"sb\\bg\\bg_honwaka.jpg\"",
            ["$R003"] = "\"sb/element/unknown.png\"",
            ["$R004"] = "\"sb\\element\\damnae-vignette-683x384.png\"",
            ["$R005"] = "\"sb\\element\\square_wall2.png\"",
            ["$R006"] = "\"sb\\element\\square_wall1.png\"",
            ["$R007"] = "\"sb\\element\\extend.png\"",
            ["$R008"] = "\"sb\\element\\dot2.png\"",
            ["$R009"] = "\"sb\\text\\Ostrich Sans_2\\e.png\"",
            ["$R010"] = "\"sb\\text\\Ostrich Sans_2\\0.png\"",
            ["$R011"] = "\"sb\\text\\Ostrich Sans_2\\1.png\"",
            ["$R012"] = "\"sb\\text\\Ostrich Sans_2\\2.png\"",
            ["$R013"] = "\"sb\\bg\\pixelsort.png\"",
            ["$R014"] = "\"sb\\element\\square_ring.png\"",
            ["$R015"] = "\"sb\\element\\circle.png\"",
            ["$R016"] = "\"sb\\element\\Hex.png\"",
            ["$R017"] = "\"sb\\element\\bar2.png\"",
            ["$R018"] = "\"sb\\element\\dot.png\"",
            ["$R019"] = "\"sb\\text\\Teko-Light\\Teko-Light-.png\"",
            ["$R020"] = "\"sb\\text\\Teko-Light\\Teko-Light-0.png\"",
            ["$R021"] = "\"sb\\text\\Teko-Light\\Teko-Light-1.png\"",
            ["$R022"] = "\"sb\\text\\Campton Book\\0072.png\"",
            ["$R023"] = "\"sb\\text\\Campton Book\\0074.png\"",
            ["$R024"] = "\"sb\\text\\Campton Book\\0069.png\"",
            ["$R025"] = "\"sb\\text\\Ostrich Sans\\0043.png\"",
            ["$R026"] = "\"sb\\text\\Ostrich Sans\\0061.png\"",
            ["$R027"] = "\"sb\\text\\Ostrich Sans\\006d.png\"",
            ["$R028"] = "\"sb\\text\\Ostrich Sans\\0065.png\"",
            ["$R029"] = "\"sb\\text\\Ostrich Sans\\006c.png\"",
            ["$R030"] = "\"sb\\text\\Ostrich Sans\\0069.png\"",
            ["$R031"] = "\"sb\\text\\Ostrich Sans\\0076.png\"",
            ["$R032"] = "\"sb\\text\\Ostrich Sans\\0073.png\"",
            ["$R033"] = "\"sb\\text\\Ostrich Sans\\0072.png\"",
            ["$R034"] = "\"sb\\text\\Ostrich Sans\\006f.png\"",
            ["$R035"] = "\"sb\\text\\Campton Book\\0065.png\"",
            ["$R036"] = "\"sb\\text\\Ostrich Sans\\0074.png\"",
            ["$R037"] = "\"sb\\text\\Ostrich Sans\\0079.png\"",
            ["$R038"] = "\"sb\\text\\Ostrich Sans\\006e.png\"",
            ["$R039"] = "\"sb\\text\\Campton Book\\0061.png\"",
            ["$R040"] = "\"sb\\text\\Campton Book\\0070.png\"",
            ["$R041"] = "\"sb\\text\\Ostrich Sans\\0075.png\"",
            ["$R042"] = "\"sb\\text\\Campton Book\\006f.png\"",
            ["$R043"] = "\"sb\\text\\title.png\"",
            ["$R044"] = "\"sb\\text\\KillGothic\\0041.png\"",
            ["$R045"] = "\"sb\\text\\KillGothic\\0049.png\"",
            ["$R046"] = "\"sb\\text\\KillGothic\\0052.png\"",
            ["$R047"] = "\"sb\\text\\Voyager Grotesque Light\\0050.png\"",
            ["$R048"] = "\"sb\\element\\firefly3.png\"",
            ["$R049"] = "\"sb\\text\\KITCHENPOLICE\\006f.png\"",
            ["$R050"] = "\"sb\\text\\KITCHENPOLICE\\0064.png\"",
            ["$R051"] = "\"sb\\text\\KITCHENPOLICE\\002e.png\"",
            ["$R052"] = "\"sb\\text\\KITCHENPOLICE\\0065.png\"",
            ["$R053"] = "\"sb\\element\\ring2.png\"",
            ["$R054"] = "\"sb\\element\\Hex2.png\"",
            ["$R055"] = "\"sb\\element\\sunflare2.png\"",
            ["$R056"] = "\"sb\\text\\imagine_font\\animation\\r\\n_.png\"",
            ["$R057"] = "\"sb\\element\\r/hud1.png\"",
            ["$R058"] = "\"sb\\element\\r/hud2.png\"",
            ["$R059"] = "\"sb\\element\\r/hud3.png\"",
            ["$R060"] = "\"sb\\element\\r/hud4.png\"",
            ["$R061"] = "\"sb\\element\\r/hud5.png\"",
            ["$R062"] = "\"sb\\element\\r/hud6.png\"",
            ["$R063"] = "\"sb\\element\\realnoise/noise_0.png\"",
            ["$R064"] = "\"sb\\text\\imagine_font\\animation\\s\\n_.png\"",
            ["$R065"] = "\"sb\\text\\DS-DIGI\\0030.png\"",
            ["$R066"] = "\"sb\\text\\DS-DIGI\\0031.png\"",
            ["$R067"] = "\"sb/element/stripe2.png\"",
            ["$R068"] = "\"sb\\text\\chogokubosogothic_5/0053_1.png\"",
            ["$R069"] = "\"sb\\text\\chogokubosogothic_5/0069_1.png\"",
            ["$R070"] = "\"sb\\text\\chogokubosogothic_5/006e_1.png\"",
            ["$R071"] = "\"sb\\text\\chogokubosogothic_5/0067_1.png\"",
            ["$R072"] = "\"sb\\text\\Eurostile LT Std Ext Two\\0050.png\"",
            ["$R073"] = "\"sb\\text\\Eurostile LT Std Ext Two\\0061.png\"",
            ["$R074"] = "\"sb\\text\\Eurostile LT Std Ext Two\\0072.png\"",
            ["$R075"] = "\"sb\\text\\Eurostile LT Std Ext Two\\0074.png\"",
            ["$R076"] = "\"sb\\element\\Fog0.png\"",
            ["$R077"] = "\"sb\\element\\Fog1.png\"",
            ["$R078"] = "\"sb\\element\\halfcircle.png\"",
            ["$R079"] = "\"sb\\element\\gradationbaseTL.png\"",
            ["$R080"] = "\"sb\\element\\gradationbaseB.png\"",
            ["$R081"] = "\"sb\\element\\gradationbaseR.png\"",
            ["$R082"] = "\"sb\\logo\\r.png\""
        }.ToList();
    }

    [Benchmark(Baseline = true)]
    public object? String_All()
    {
        var text = File.ReadAllText(_path);
        text = ParseVariables(text);

        var sr = new StringReader(text);
        var line = sr.ReadLine();

        var sb = new StringBuilder();
        while (line != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // do some stuffs
            sb.AppendLine(line);
            // do some stuffs

            line = sr.ReadLine();
        }

        return sb.ToString();
    }

    [Benchmark]
    public object? StringBuilder_All()
    {
        var readAllText = File.ReadAllText(_path);
        var gg = new StringBuilder(readAllText);
        ParseVariables(gg);
        var text = gg.ToString();

        var sr = new StringReader(text);
        var line = sr.ReadLine();

        var sb = new StringBuilder();
        while (line != null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // do some stuffs
            sb.AppendLine(line);
            // do some stuffs

            line = sr.ReadLine();
        }

        return sb.ToString();
    }

    [Benchmark]
    public object? StringBuilder_MultiLines()
    {
        var sb = new StringBuilder();
        using var sw = new StreamReader(_path);
        while (!sw.EndOfStream)
        {
            var line = sw.ReadLine();

            if (string.IsNullOrWhiteSpace(line)) continue;

            var s = new StringBuilder(line);
            ParseVariables(s);

            var value = s.ToString();

            // do some stuffs
            sb.AppendLine(value);
            // do some stuffs
        }

        return sb.ToString();
    }

    [Benchmark]
    public object? String_MultiLines()
    {
        var sb = new StringBuilder();
        using var sw = new StreamReader(_path);
        while (!sw.EndOfStream)
        {
            var line = sw.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var value = ParseVariables(line);

            // do some stuffs
            sb.AppendLine(value);
            // do some stuffs
        }

        return sb.ToString();
    }


    private string ParseVariables(string line)
    {
        for (var i = 0; i < _dic.Count; i++)
        {
            var v = _dic[i];
            line = line.Replace(v.Key, v.Value);
        }

        return line;
    }

    private void ParseVariables(StringBuilder sb)
    {
        for (var i = 0; i < _dic.Count; i++)
        {
            var v = _dic[i];
            sb.Replace(v.Key, v.Value);
        }
    }
}