namespace Todo.Core.Tests
{
    public class TodoListTests
    {
        [Fact]
        public void Add_IncreasesCount()
        {
            var list = new TodoList();
            list.Add("  task  ");
            Assert.Equal(1, list.Count);
            Assert.Equal("task", list.Items.First().Title);
        }

        [Fact]
        public void Remove_ById_Works()
        {
            var list = new TodoList();
            var item = list.Add("a");
            Assert.True(list.Remove(item.Id));
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void Find_ReturnsMatches()
        {
            var list = new TodoList();
            list.Add("Buy milk");
            list.Add("Read book");
            var found = list.Find("buy").ToList();
            Assert.Single(found);
            Assert.Equal("Buy milk", found[0].Title);
        }
        
        [Fact]
        public void Save_ShouldCreateJsonFile()
        {
            var todoList = new TodoList();
            todoList.Add("Task 1");
            todoList.Add("Task 2");
            var tempFile = Path.GetTempFileName();

            try
            {
                todoList.Save(tempFile);
                
                Assert.True(File.Exists(tempFile));
                var fileContent = File.ReadAllText(tempFile);
                Assert.Contains("Task 1", fileContent);
                Assert.Contains("Task 2", fileContent);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_ShouldRestoreTodoList()
        {
            var originalList = new TodoList();
            var item1 = originalList.Add("Task 1");
            var item2 = originalList.Add("Task 2");
            item2.MarkDone();
            
            var tempFile = Path.GetTempFileName();

            try
            {
                originalList.Save(tempFile);
                
                var loadedList = TodoList.Load(tempFile);
                
                Assert.Equal(2, loadedList.Count);
                Assert.Equal("Task 1", loadedList.Items[0].Title);
                Assert.Equal("Task 2", loadedList.Items[1].Title);
                Assert.False(loadedList.Items[0].IsDone);
                Assert.True(loadedList.Items[1].IsDone);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveAndLoad_ShouldPreserveItemProperties()
        {
            var originalList = new TodoList();
            var item = originalList.Add("Test Task");
            item.MarkDone();
            var originalId = item.Id;
            
            var tempFile = Path.GetTempFileName();

            try
            {
                originalList.Save(tempFile);
                var loadedList = TodoList.Load(tempFile);
                var loadedItem = loadedList.Items.First();
                
                Assert.Equal(originalId, loadedItem.Id);
                Assert.Equal("Test Task", loadedItem.Title);
                Assert.True(loadedItem.IsDone);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void Load_NonExistentFile_ShouldThrowException()
        {
            var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent.json");
            
            Assert.Throws<FileNotFoundException>(() => TodoList.Load(nonExistentFile));
        }

        [Fact]
        public void Save_EmptyList_ShouldCreateValidJson()
        {
            var emptyList = new TodoList();
            var tempFile = Path.GetTempFileName();

            try
            {
                emptyList.Save(tempFile);
                
                var fileContent = File.ReadAllText(tempFile);
                Assert.Equal("[]", fileContent.Trim());
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}