using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Assets.ObjAnimations
{
    public class ObjMenuItems : EditorWindow
    {
        [MenuItem("Obj Extensions/Process Animations")]
        static void Init()
        {
            var path = EditorUtility.SaveFolderPanel("ObjAnimations", Application.dataPath, "");

            var parsedValues = ObjMeshParser.GetAnimationFrames(path);

            var anim = CreateInstance<ObjAnimationSO>();

            var objAnimationBakedFrames = parsedValues as ObjAnimationBakedFrame[] ?? parsedValues.ToArray();

            anim.Vertices = objAnimationBakedFrames.SelectMany(x=> x.Vertices.Select(y => (Vector3)y)).ToArray();

            anim.Indices = objAnimationBakedFrames.SelectMany(x=>x.Indices.Select(y => y - 1)).ToArray();

            anim.Normals = objAnimationBakedFrames.SelectMany(x=>x.Vertices.Select(y => (Vector3) y)).ToArray();

            anim.VerticesPerMesh = objAnimationBakedFrames[0].Vertices.Length;

            anim.IndicesPerMesh = objAnimationBakedFrames[0].Indices.Length;

            anim.SubMeshCount = objAnimationBakedFrames.Length;

            anim.CreateAsset(path, "New Obj Animation");
        }


    }
}
