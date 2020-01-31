using UnityEngine;

namespace Util
{
    public class ExecutableBuilderTool : MonoBehaviour
    {
        [SerializeField] private string[] scenes;
        [SerializeField] private string buildsFolder;
        [SerializeField] private string executableName;
        [SerializeField] private bool runAfterBuild;

        public string[] Scenes
        {
            get { return scenes; }
        }

        public string BuildsFolder
        {
            get { return buildsFolder; }
        }

        public string ExecutableName
        {
            get { return executableName; }
        }

        public bool RunAfterBuild
        {
            get { return runAfterBuild; }
        }

        // TODO
        public bool PackAfterBuild
        {
            get { return false; }
        }
    }
}