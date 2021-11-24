using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Beatmap;
using Coosu.Storyboard;
using Coosu.Storyboard.Storybrew.Text;
using Newtonsoft.Json.Linq;
using StorybrewCommon.Scripting;

namespace StorybrewScriptTest
{
    public struct BookmarkObj
    {
        public BookmarkObj(int lead, int fadeIn, int fadeOut)
        {
            Lead = lead;
            FadeIn = fadeIn;
            FadeOut = fadeOut;
        }

        public int Lead { get; set; }
        public int FadeIn { get; set; }
        public int FadeOut { get; set; }
    }

    public class MetaEffect : StoryboardObjectGenerator
    {
        private const int x = 610;
        public override void Generate()
        {
            var layer = new Layer();
            var secTitle = layer.CreateText("~ Title ~", 0, x, 50, ConfigSection());
            FixSection(secTitle);
            var secSource = layer.CreateText("~ Source ~", 0, x, 155, ConfigSection());
            FixSection(secSource);
            var secBpm = layer.CreateText("~ BPM ~", 0, x, 260, ConfigSection());
            FixSection(secBpm);
            var secBpmMin = layer.CreateText("MIN", 0, x - 100, 260, ConfigSubSection());
            FixSection(secBpmMin);
            var secBpmMax = layer.CreateText("MAX", 0, x + 100, 260, ConfigSubSection());
            FixSection(secBpmMax);
            var secMapper = layer.CreateText("~ Mapper ~", 0, x, 365, ConfigSection());
            FixSection(secMapper);

            var obj = OsuFile
                .ReadFromFileAsync(
                    @"E:\Games\osu!\Songs\1338258 Shimotsuki Haruka - Songs Compilation\Shimotsuki Haruka - Songs Compilation (Gust) [bookmark].osu")
                .Result;
            var actualBookmarks = obj.Editor.Bookmarks;
            var bookmarks = new List<BookmarkObj>();
            int i = 0;
            int tmpLead = 0;
            int tmpFadeIn = 0;
            int tmpFadeOut = 0;
            foreach (var bm in actualBookmarks)
            {
                if (i == 0) tmpLead = bm;
                else if (i == 1) tmpFadeIn = bm;
                else if (i == 2)
                {
                    tmpFadeOut = bm;
                    i = 0;
                    var bmObj = new BookmarkObj(tmpLead, tmpFadeIn, tmpFadeOut);
                    bookmarks.Add(bmObj);
                    tmpLead = 0;
                    tmpFadeIn = 0;
                    tmpFadeOut = 0;
                    continue;
                }

                i++;
            }

            // Log(JsonConvert.SerializeObject(bookmarks, Formatting.Indented));
            SetValue(layer, bookmarks);
        }

        private void SetValue(Layer layer, List<BookmarkObj> bookmarks)
        {
            var text = File.ReadAllText(Path.Combine(ProjectPath, "info.json"));
            JArray jArr = Newtonsoft.Json.Linq.JArray.Parse(text);

            var titleStrs = new List<string>();
            var sourceStrs = new List<string>();
            var mapperStrs = new List<string>();

            foreach (var jToken in jArr)
            {
                var jObj = (JObject)jToken;
                var titleStr = jObj["title"].Value<string>();
                var sourceStr = jObj["source"].Value<string>();
                var mapperStr = jObj["mapper"].Value<string>();

                titleStrs.Add(titleStr);
                sourceStrs.Add(sourceStr);
                mapperStrs.Add(mapperStr);
            }

            for (int i = 0; i < bookmarks.Count; i++)
            {
                var bmObj = bookmarks[i];
                var titleStr = titleStrs[i];
                var sourceStr = sourceStrs[i];
                var mapperStr = mapperStrs[i];

                var title = layer.CreateText(titleStr, bmObj.FadeIn, x, 95, k =>
                {
                    GeneralConfigContent(k);
                });
                FixContentSection(title, bmObj.FadeIn, bmObj.FadeOut);

                var source = layer.CreateText(sourceStr, bmObj.FadeIn, x, 200, k =>
                {
                    GeneralConfigContent(k);
                    k.WithWordGap(-3);
                    if (i == 0)
                    {
                        k.ScaleXBy(0.55);
                        k.ScaleYBy(0.65);
                    }
                    else if (i == 1)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 2)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 3)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 4)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 5)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 6)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 7)
                    {
                        k.ScaleXBy(0.48);
                        k.ScaleYBy(0.65);
                    }
                    else if (i == 8)
                    {
                        k.ScaleXBy(0.48);
                        k.ScaleYBy(0.65);
                    }
                    else if (i == 9)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 10)
                    {
                        k.ScaleXBy(0.7);
                        k.ScaleYBy(0.7);
                    }
                    else if (i == 11)
                    {
                        k.ScaleXBy(0.58);
                        k.ScaleYBy(0.65);
                    }
                    else if (i == 12)
                    {
                        k.ScaleXBy(0.48);
                        k.ScaleYBy(0.65);
                    }
                    else if (i == 13)
                    {
                        k.ScaleXBy(0.45);
                        k.ScaleYBy(0.58);
                    }
                });
                FixContentSection(source, bmObj.FadeIn, bmObj.FadeOut);

                var bpm = layer.CreateText("0", bmObj.FadeIn, x, 305, k =>
                {
                    GeneralConfigContent2(k);
                });
                FixContentSection(bpm, bmObj.FadeIn, bmObj.FadeOut);

                var mapper = layer.CreateText(mapperStr, bmObj.FadeIn, x, 410, k =>
                {
                    GeneralConfigContent2(k);
                    k.WithWordGap(-1);
                    k.ScaleXBy(0.6);
                });
                FixContentSection(mapper, bmObj.FadeIn, bmObj.FadeOut);

            }

            // var title = layer.CreateText("白夜幻想譚", 0, x, 95, k =>
            // {
            //     GeneralConfigContent(k);
            // });
            // FixSection(title);

            // var source = layer.CreateText("イリスのアトリエ エターナルマナ", 0, x, 200, k =>
            // {
            //     GeneralConfigContent(k);
            //     k.WithWordGap(-3);
            //     k.ScaleXBy(0.6);
            // });
            // FixSection(source);

            // var bpm = layer.CreateText("128", 0, x, 305, k =>
            // {
            //     GeneralConfigContent2(k);
            // });
            // FixSection(bpm);

            // var mapper = layer.CreateText("Gust", 0, x, 410, k =>
            // {
            //     GeneralConfigContent2(k);
            // });
            // FixSection(mapper);


            layer.ExecuteBrew(this);
        }

        private void FixContentSection(SpriteGroup group, int fadeIn, int fadeOut)
        {
            foreach (var item in group)
            {
                item.Fade(2, fadeIn - 200, fadeIn, 0, 1);
                item.Fade(fadeIn, fadeOut, 1);
                item.Fade(0, fadeOut, fadeOut + 3000, 1, 0);
            }
        }

        private static void FixSection(SpriteGroup group)
        {
            foreach (var item in group)
            {
                item.Fade(0, 1294775, 1);
            }
        }

        private static void GeneralConfigContent2(CoosuTextOptionsBuilder k)
        {
            k.WithIdentifier("content2");
            k.WithFontFamily("SWSimp");
            k.ScaleXBy(0.7);
            k.ScaleYBy(0.7);
            k.WithFontSize(32);
            k.WithShadow("#80FFFFFF", 15, depth: 0);
        }

        private static void GeneralConfigContent(CoosuTextOptionsBuilder k)
        {
            k.WithIdentifier("content");
            k.WithFontFamily("BIZ UDMincho");
            k.ScaleXBy(0.7);
            k.ScaleYBy(0.7);
            k.WithFontSize(30);
            k.WithShadow("#80FFFFFF", 15, depth: 0);
        }

        private static Action<CoosuTextOptionsBuilder> ConfigSubSection()
        {
            return k =>
            {
                k.WithIdentifier("subsection");
                k.WithFontFamily("SWSimp");
                k.ScaleXBy(0.7);
                k.ScaleYBy(0.7);
                k.WithFontSize(22);
                k.WithShadow("#80FFFFFF", 15, depth: 0);
            };
        }

        private static Action<CoosuTextOptionsBuilder> ConfigSection()
        {
            return k =>
            {
                k.WithIdentifier("section");
                k.WithFontFamily("SWSimp");
                k.ScaleXBy(0.7);
                k.ScaleYBy(0.7);
                k.WithFontSize(34);
                k.WithShadow("#80FFFFFF", 15, depth: 0);
            };
        }
    }
}
