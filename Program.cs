using System.Text.Json;
using System.Text.Json.Serialization;
using Lab_3_Kashin;

bool working = true;
while (working)
{
    try
    {
        FixedArr arr1 = null;
        Console.WriteLine("1 - Ручний ввід");
        Console.WriteLine("2 - Завантажити з JSON");
        Console.Write("Оберіть режим вводу для першого масиву: ");
        string mode = Console.ReadLine();
        int n = 0;
        if (mode == "2")
        {
            string fileName = "arr1.json";
            if (File.Exists(fileName))
            {
                arr1 = FixedArr.LoadFromJson(fileName);
                n = arr1.GetLength();
                Console.WriteLine($"Перший масив завантажено з файлу {fileName} (розмір: {n})."); 
                arr1.DisplayArr();
            }
            else
            {
                Console.WriteLine($"Файл {fileName} не знайдено. Буде використано ручний ввід.");
                Console.WriteLine("Введіть розмір масиву:");
                n = int.Parse(Console.ReadLine());
                arr1 = ReadArrayFromConsole(n, "першого масиву");
            }
        }
        else
        {
            Console.WriteLine("Введіть розмір масиву:");
            n = int.Parse(Console.ReadLine());
            arr1 = ReadArrayFromConsole(n, "першого масиву");
        }

        Console.WriteLine();
        FixedArr arr2 = ReadArrayFromConsole(n, "другого масиву");
        Console.WriteLine();

        FixedArr connectedArr = arr1.Connector(arr2);
        Console.WriteLine("Результат поелементного зчеплення двох масивів:");
        connectedArr.DisplayArr();
        Console.WriteLine();

        FixedArr mergedArr = FixedArr.Merge(arr1, arr2);
        Console.WriteLine("Результат злиття двох масивів без повторів:");
        mergedArr.DisplayArr();
        Console.WriteLine();

        Console.Write("Введіть індекс елемента для виведення з першого масиву: ");
        int index = int.Parse(Console.ReadLine());
        Console.Write("Елемент за заданим індексом: ");
        arr1.DisplayElement(index);

        Console.WriteLine("Перший масив:");
        arr1.DisplayArr();

        Console.WriteLine("\nЗберегти перший масив у JSON файл? (y/n)");
        if (Console.ReadLine() == "y")
        {
            string fileName = "arr1.json";
            arr1.SaveToJson(fileName);
            Console.WriteLine($"Масив збережено у файл {fileName}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    Console.WriteLine();
    Console.WriteLine("Повторити виконання програми? (y/n)");
    string choice = Console.ReadLine();
    if (choice != "y")
        working = false;

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("\n------------------------");
}

static FixedArr ReadArrayFromConsole(int size, string arrName)
{
    FixedArr arr = new FixedArr(size);
    Console.WriteLine($"Введіть елементи {arrName}:");
    for (int i = 0; i < size; i++)
    {
        Console.Write($"Елемент [{i}]: ");
        arr[i] = Console.ReadLine();
    }
    return arr;
}



namespace Lab_3_Kashin
{
    public class FixedArr
    {
        [JsonInclude]
        private string[] _array;

        public FixedArr(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Розмір масиву має бути більше нуля.");
            }
            _array = new string[size];
        }

        [JsonConstructor]
        public FixedArr(string[] _array)
        {
            if (_array == null)
            {
                throw new ArgumentNullException("_array");
            }
            this._array = _array;
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= _array.Length)
                {
                    throw new IndexOutOfRangeException("Індекс виходить за межі масиву.");
                }
                return _array[index];
            }
            set
            {
                if (index < 0 || index >= _array.Length)
                {
                    throw new IndexOutOfRangeException("Індекс виходить за межі масиву.");
                }
                _array[index] = value;
            }
        }

        public FixedArr Connector(FixedArr other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (_array.Length != other._array.Length)
            {
                throw new ArgumentException("Масиви мають бути однакової довжини для поелементного зчеплення.");
            }

            FixedArr result = new FixedArr(_array.Length);
            for (int i = 0; i < _array.Length; i++)
            {
                string first = this[i];
                string second = other[i];

                if (first == null)
                {
                    first = "";
                }
                if (second == null)
                {
                    second = "";
                }

                result[i] = first + second;
            }
            return result;
        }

        public static FixedArr Merge(FixedArr arr1, FixedArr arr2)
        {
            if (arr1 == null || arr2 == null)
            {
                throw new ArgumentNullException("Масив не може бути null.");
            }

            List<string> list = new List<string>();

            foreach (var item in arr1._array)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
            foreach (var item in arr2._array)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }

            FixedArr result = new FixedArr(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = list[i];
            }
            return result;
        }


        public void DisplayElement(int index)
        {
            Console.WriteLine(this[index]);
        }

        public void DisplayArr()
        {
            for (int i = 0; i < _array.Length; i++)
            {
                Console.WriteLine($"[{i}] {_array[i]}");
            }
        }

        public int GetLength()
        {
            return _array.Length;
        }

        public void SaveToJson(string fileName)
        {
            var options = new JsonSerializerOptions { IncludeFields = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(fileName, json);
        }

        public static FixedArr LoadFromJson(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"Файл {fileName} не знайдено.");
            }
            string json = File.ReadAllText(fileName);
            var options = new JsonSerializerOptions { IncludeFields = true };
            return JsonSerializer.Deserialize<FixedArr>(json, options);
        }
    }
}
