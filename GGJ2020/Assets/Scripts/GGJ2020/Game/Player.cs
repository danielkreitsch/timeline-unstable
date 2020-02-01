using UnityEngine;

namespace GGJ2020.Game
{
    public class Player : MonoBehaviour
    {
        private string name;
        [SerializeField] private Board board;

        public void Init(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public Board Board => board;
    }
}