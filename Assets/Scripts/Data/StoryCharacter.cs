using UnityEngine;
using System.Collections.Generic;

namespace CoinsOfHope.Data
{
    [CreateAssetMenu(fileName = "StoryCharacter", menuName = "Coins of Hope/Story Character")]
    public class StoryCharacter : ScriptableObject
    {
        [Header("Character Info")]
        public string storyId;
        public string characterName;
        
        [TextArea(3, 6)]
        public string introText;
        
        [TextArea(3, 6)]
        public string resolutionText;
        
        public Sprite characterImage;

        [Header("Needs")]
        public List<CharacterNeed> needs = new List<CharacterNeed>();

        public int GetTotalCost()
        {
            int total = 0;
            foreach (var need in needs)
            {
                total += need.cost;
            }
            return total;
        }
    }

    [System.Serializable]
    public class CharacterNeed
    {
        public string needId;
        public string needName;
        public string description;
        public int cost;
        public Sprite icon;
    }
}
