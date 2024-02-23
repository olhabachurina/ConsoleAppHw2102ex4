namespace ConsoleAppHw2102ex4;

class Program
{

    static Mutex mutex = new Mutex();

    static void Main(string[] args)
    {
        // Генерация случайных чисел и запись их в файл
        Thread generatorThread = new Thread(() => GenerateNumbers("numbers.txt", 100));
        generatorThread.Start();

        // Анализ содержимого файла и создание нового файла с простыми числами
        Thread primeThread = new Thread(() => FindAndWritePrimes("numbers.txt", "primes.txt"));
        primeThread.Start();

        // Ожидание завершения работы анализирующего потока и фильтрация чисел с последней цифрой 7
        primeThread.Join();
        Thread filterThread = new Thread(() => FilterNumbers("primes.txt", "filtered_primes.txt"));
        filterThread.Start();
    }

    static void GenerateNumbers(string filename, int n)
    {
        mutex.WaitOne();
        using (StreamWriter writer = new StreamWriter(filename))
        {
            Random rand = new Random();
            for (int i = 0; i < n; i++)
            {
                writer.WriteLine(rand.Next(1, 1000));
            }
        }
        mutex.ReleaseMutex();
        Console.WriteLine($"Сгенерированы и записаны {n} случайных чисел в файл {filename}");
    }

    static bool IsPrime(int num)
    {
        if (num <= 1) return false;
        for (int i = 2; i * i <= num; i++)
        {
            if (num % i == 0) return false;
        }
        return true;
    }

    static void FindAndWritePrimes(string inputFilename, string outputFilename)
    {
        mutex.WaitOne();
        List<int> primes = new List<int>();
        using (StreamReader reader = new StreamReader(inputFilename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                int num = int.Parse(line);
                if (IsPrime(num))
                {
                    primes.Add(num);
                }
            }
        }
        using (StreamWriter writer = new StreamWriter(outputFilename))
        {
            foreach (int prime in primes)
            {
                writer.WriteLine(prime);
            }
        }
        mutex.ReleaseMutex();
        Console.WriteLine($"Найдены и записаны простые числа в файл {outputFilename}");
    }

    static void FilterNumbers(string inputFilename, string outputFilename)
    {
        mutex.WaitOne();
        List<int> filteredNumbers = new List<int>();
        using (StreamReader reader = new StreamReader(inputFilename))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                int num = int.Parse(line);
                if (num % 10 == 7)
                {
                    filteredNumbers.Add(num);
                }
            }
        }
        using (StreamWriter writer = new StreamWriter(outputFilename))
        {
            foreach (int num in filteredNumbers)
            {
                writer.WriteLine(num);
            }
        }
        mutex.ReleaseMutex();
        Console.WriteLine($"Отфильтрованы и записаны числа с последней цифрой 7 в файл {outputFilename}");
    }
}