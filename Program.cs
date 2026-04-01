// Точка входа в программу - демонстрирует работу паттерна Observer
class Program
{
    static void Main(string[] args)
    {
        // Создаём субъекта - объект, за которым наблюдают
        PerformanceSubject perf = new PerformanceSubject();

        // Создаём наблюдателей и сразу регистрируем их в субъекте
        // Деканат и кафедра будут автоматически получать уведомления об изменениях успеваемости
        DeansOffice deans = new DeansOffice("Деканат СПИНТех", perf);
        Department department = new Department("Кафедра СПИНТех", perf);

        // Событие 1: преподаватель обновил данные - все наблюдатели получат уведомление
        Console.WriteLine("Неделя 1: Преподаватель разместил текущую успеваемость в базе");
        perf.TeacherUpdate("ПИН-35", "ПИАПС", 42);

        // Событие 2: преподаватель не обновил данные вовремя - наблюдатели получат уведомление о нарушении
        Console.WriteLine("\nНеделя 2: Преподаватель не разместил успеваемость вовремя");
        perf.EndOfWeekNoUpdate("ПИН-35", "ПИАПС");
    }
}

// Интерфейс наблюдателя - определяет метод, который вызывается при обновлении данных субъекта
// Принимает объект с данными об успеваемости
interface IObserver
{
    void Update(Object ob);
}

// Интерфейс субъекта (наблюдаемого объекта) - определяет методы управления подпиской наблюдателей
// Позволяет наблюдателям регистрироваться, отписываться и получать массовые уведомления
interface IObservable
{
    void RegisterObserver(IObserver o);
    void RemoveObserver(IObserver o);
    void NotifyObservers();
}

// Конкретный субъект - хранит состояние успеваемости и управляет списком наблюдателей
// При изменении данных автоматически уведомляет всех зарегистрированных наблюдателей
class PerformanceSubject : IObservable
{
    private PerformanceInfo pInfo;
    private List<IObserver> observers;

    public PerformanceSubject()
    {
        observers = new List<IObserver>();
        pInfo = new PerformanceInfo();
    }

    // Добавляет наблюдателя в список подписчиков
    public void RegisterObserver(IObserver o)
    {
        observers.Add(o);
    }

    // Удаляет наблюдателя из списка подписчиков
    public void RemoveObserver(IObserver o)
    {
        observers.Remove(o);
    }

    // Проходит по всем зарегистрированным наблюдателям и вызывает их метод Update
    // Передаёт текущее состояние успеваемости (объект PerformanceInfo)
    public void NotifyObservers()
    {
        foreach (IObserver o in observers)
        {
            o.Update(pInfo);
        }
    }

    // Метод вызывается преподавателем при размещении успеваемости вовремя
    // Обновляет данные и уведомляет всех наблюдателей
    public void TeacherUpdate(string group, string discipline, double score)
    {
        pInfo.Group = group;
        pInfo.Discipline = discipline;
        pInfo.AverageScore = score;
        pInfo.OnTime = true;
        NotifyObservers();
    }

    // Метод вызывается в конце недели, если преподаватель не разместил успеваемость
    // Устанавливает флаг нарушения срока и уведомляет всех наблюдателей
    public void EndOfWeekNoUpdate(string group, string discipline)
    {
        pInfo.Group = group;
        pInfo.Discipline = discipline;
        pInfo.AverageScore = 0;
        pInfo.OnTime = false;
        NotifyObservers();
    }
}

// Класс данных - содержит информацию об успеваемости
// Используется для передачи состояния от субъекта к наблюдателям
class PerformanceInfo
{
    public string Group { get; set; }
    public string Discipline { get; set; }
    public double AverageScore { get; set; }
    public bool OnTime { get; set; }
}

// Конкретный наблюдатель - деканат
// Отслеживает успеваемость и реагирует на нарушения сроков размещения данных преподавателем
class DeansOffice : IObserver
{
    public string Name { get; set; }
    private IObservable perf;

    // При создании деканат регистрируется как наблюдатель в субъекте
    public DeansOffice(string name, IObservable obs)
    {
        this.Name = name;
        perf = obs;
        perf.RegisterObserver(this);
    }

    // Метод вызывается автоматически при обновлении данных в субъекте
    // Проверяет флаг своевременности и выводит соответствующее сообщение
    public void Update(object ob)
    {
        PerformanceInfo info = (PerformanceInfo)ob;

        if (info.OnTime)
        {
            Console.WriteLine($"{Name} отслеживает: группа {info.Group}, дисциплина \"{info.Discipline}\", средний балл = {info.AverageScore}");
        }
        else
        {
            Console.WriteLine($"{Name}: Преподаватель не создал успеваемость вовремя для группы {info.Group} по дисциплине \"{info.Discipline}\"!");
            Console.WriteLine($"{Name} оповещает кафедру о нарушении срока.");
        }
    }
}

// Конкретный наблюдатель - кафедра
// Получает уведомления о размещении успеваемости и о нарушениях сроков от деканата
class Department : IObserver
{
    public string Name { get; set; }
    private IObservable perf;

    // При создании кафедра регистрируется как наблюдатель в субъекте
    public Department(string name, IObservable obs)
    {
        this.Name = name;
        perf = obs;
        perf.RegisterObserver(this);
    }

    // Метод вызывается автоматически при обновлении данных в субъекте
    // Реагирует на успешное размещение данных или на уведомление о нарушении
    public void Update(object ob)
    {
        PerformanceInfo info = (PerformanceInfo)ob;

        if (info.OnTime)
        {
            Console.WriteLine($"{Name}: Получена обновлённая успеваемость по группе {info.Group} ({info.Discipline}).");
        }
        else
        {
            Console.WriteLine($"{Name}: Получено оповещение от деканата - преподаватель не разместил данные вовремя для группы {info.Group}.");
        }
    }
}