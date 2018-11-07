using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.ObjAnimations
{
    public class ObjMeshParser
    {
        public static IEnumerable<ObjAnimationBakedFrame> GetAnimationFrames(string directoryPath)
        {
            var extensionPattern = new Regex(@"\.obj$");

            var files = Directory.GetFiles(directoryPath);

            var data = new Dictionary<int, ObjAnimationBakedFrame>();

            foreach (var file in files)
            {
                if (!extensionPattern.IsMatch(file))
                    continue;

                var frameNumber = ExtractFrameNumber(file);

                var lines = File.ReadAllText(file);

                var bakedFrame = GetFrame(lines);

                data.Add(frameNumber, bakedFrame);
            }

            return data.OrderBy(x => x.Key).Select(x => x.Value);
        }


        private static ObjAnimationBakedFrame GetFrame(string fileLines)
        {
            var result = new List<Vector3>();
            var indices = new List<int>();

            var regex = new Regex(@"(?<vertices>v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*))|(?<indices>((?<i>\d)\/\/\d\s?))");
            var matches = regex.Matches(fileLines);


            foreach (Match match in matches)
            {
                if (!match.Success)
                    continue;

                if (match.Groups["indices"].Success)
                {
                    var i = match.Groups["i"].Captures;
                    foreach (Capture capture in i)
                    {
                        var temp = 0;
                        if (!int.TryParse(capture.Value, out temp))
                        {
                            throw new FormatException("Indice has bad format " + capture.Value);
                        }
                        indices.Add(temp);
                    }
                }

                if (match.Groups["vertices"].Success)
                {
                    var xR = SanitizeFormat(match.Groups["x"].Value);
                    var yR = SanitizeFormat(match.Groups["y"].Value);
                    var zR = SanitizeFormat(match.Groups["z"].Value);

                    float x, y, z;

                    ParseCoord(xR, out x);
                    ParseCoord(yR, out y);
                    ParseCoord(zR, out z);


                    result.Add(new Vector3(x, y, z));
                }


            }

            return new ObjAnimationBakedFrame
            {
                Vertices = result.ToArray(),
                Indices = indices.ToArray()
            };
        }

        private static void ParseCoord(string input, out float val)
        {
            if (!float.TryParse(input, out val))
            {
                throw new FormatException("This value  " + input);
            }
        }

        private static string SanitizeFormat(string input)
        {
            return input.Replace(".", ",").Trim();
        }

        private static int ExtractFrameNumber(string file)
        {
            var filename = Path.GetFileNameWithoutExtension(file);

            var split = filename.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);


            if (split.Length != 2)
                throw new ArgumentException("Invalid filename " + file);

            var number = 0;

            if (!int.TryParse(split[1], out number))
                throw new FormatException("Filename has invalid format " + split[1] + " " + file);
            return number;
        }

    }
}
