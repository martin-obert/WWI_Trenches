using System.Linq;
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

            var anim = CreateInstance<ObjAnimationSo>();

            anim.FrameMeshes = parsedValues.Select(x => new Mesh
            {
                vertices = x.Vertices,
                triangles = x.Indices
            }).ToArray();

            anim.CreateAsset(path, "New Obj Animation");
        }
    }
}
