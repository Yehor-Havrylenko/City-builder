using System.Collections.Generic;
using BuildingSystems.ManagerInheritors;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NPC
{
    public class Creator : MonoBehaviour
    {
        [SerializeField] private Controller prefab;
        [SerializeField] private Tilemap roadTilemap;
        [SerializeField] private Road road;
        private List<Controller> characters = new();

        public void Spawn(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var character = Instantiate(prefab);
                character.Initialize(roadTilemap, road.GetRandomRoadPosition());
                characters.Add(character);
            }
        }

        public void RemoveCharacter(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var character = characters[i];
                characters.Remove(character);
                Destroy(character.gameObject);
            }
        }

        public void RemoveAllCharacters()
        {
            foreach (var character in characters)
            {
                Destroy(character.gameObject);
            }
            characters.Clear();
        }
    }
}