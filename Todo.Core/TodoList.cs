using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Todo.Core
{
    public class TodoList
    {
        private readonly List<TodoItem> _items = new();
        public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();

        public TodoItem Add(string title)
        {
            var item = new TodoItem(title);
            _items.Add(item);
            return item;
        }

        public bool Remove(Guid id) => _items.RemoveAll(i => i.Id == id) > 0;

        public IEnumerable<TodoItem> Find(string substring) =>
            _items.Where(i => i.Title.Contains(substring ?? string.Empty,
                StringComparison.OrdinalIgnoreCase));

        public int Count => _items.Count;

        // Сохранение в файле JSON
        public void Save(string path)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var json = JsonSerializer.Serialize(_items, options);
            File.WriteAllText(path, json);
        }
        
        // Загрузка из JSON файла
        public static TodoList Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: {path}");

            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var items = JsonSerializer.Deserialize<List<TodoItem>>(json, options);
            
            var todoList = new TodoList();
            if (items != null)
            {
                foreach (var item in items)
                {
                    todoList._items.Add(item);
                }
            }
            
            return todoList;
        }
    }
}
