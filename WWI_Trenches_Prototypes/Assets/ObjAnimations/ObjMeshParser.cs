using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

                using (var fileStream = File.Open(file, FileMode.Open))
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        var bakedFrame = GetFrame(streamReader);

                        data.Add(frameNumber, bakedFrame);
                    }
                        
                }
                    
            }

            return data.OrderBy(x => x.Key).Select(x => x.Value);
        }

        private static Vector3 ParseV3(string[] parts)
        {

            var xR = SanitizeFormat(parts[0]);
            var yR = SanitizeFormat(parts[1]);
            var zR = SanitizeFormat(parts[2]);

            float x, y, z;

            ParseCoord(xR, out x);
            ParseCoord(yR, out y);
            ParseCoord(zR, out z);

            return new Vector3(x,y,z);
        }
        private static ObjAnimationBakedFrame GetFrame(StreamReader file)
        {
            var vectors = new List<Vector3>();
            var normals = new List<Vector3>();
            var indices = new List<int>();
            
            //var regex = new Regex(@"(?<vertices>v (?<x>-?\d+\.?\d*) (?<y>-?\d+\.?\d*) (?<z>-?\d+\.?\d*))|(?<indices>((?<i>\d)\/\/\d\s?))");
            //var matches = regex.Matches(fileLines);
            string fileLine = null;
            while ((fileLine = file.ReadLine()) != null)
            {
                
           
                var parts = fileLine.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray();

                if (fileLine.StartsWith("v "))
                {
                    if(parts.Length != 3)
                        throw new FormatException(fileLine + " not vector line");
                    vectors.Add(ParseV3(parts));
                }
                else if (fileLine.StartsWith("vn "))
                {
                    if (parts.Length != 3)
                        throw new FormatException(fileLine + " not normal line");

                    normals.Add(ParseV3(parts));
                }
                else if (fileLine.StartsWith("f "))
                {

                    foreach (var part in parts)
                    {
                        var facePoint = part.Split('/');

                        if(facePoint.Length != 3)
                            throw new ArgumentOutOfRangeException("Invalid face format " + part);

                        indices.Add(int.Parse(facePoint[0]));
                    }
                }
            }

            if(vectors.Count != normals.Count)
                throw new ArgumentOutOfRangeException(vectors.Count + "!=" + normals.Count );
            return new ObjAnimationBakedFrame
            {
                Vertices = vectors.ToArray(),
                Normals = normals.ToArray(),
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


            if (split.Length < 2)
                throw new ArgumentException("Invalid filename " + file);

            var number = 0;

            if (!int.TryParse(split[split.Length - 1], out number))
                throw new FormatException("Filename has invalid format " + split[split.Length-1] + " " + file);
            return number;
        }

    }
}
