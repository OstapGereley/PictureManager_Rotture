using System.Collections.Generic;

namespace PictureManager.Models
{
    class Category
    {
        public string name;
        public List<string> tags;
        public int priority; // 0 - найвысший приоритет

        public Category(string name)
        {
            this.name = name;
            priority = 0;
        }

        public void ChangePriority(int priority) // вызывается при перемещении категории в стеке                                                // и получает аргумент в зависимости от нового положения в стеке
        {
            this.priority = priority;
        }

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }

        public void DeleteTag(string tag)
        {
            tags.Remove(tag);
        }


    
    }
}
