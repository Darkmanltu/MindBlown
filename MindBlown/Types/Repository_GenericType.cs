using MindBlown.Interfaces;

namespace MindBlown.Types;
public class Repository<T> where T : class, IHasGuidId
{
    private readonly List<T> _items = new List<T>();

    public Repository()
    {
        _items = new List<T>(); // Initialize with an empty list
    }

    public Repository(List<T> items)
    {
        if (items != null)
        {
            _items.AddRange(items);
        }
    }

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
    }

    public T? GetById(Guid id)
    {
        return _items.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return _items;
    }
    public int Count()
    {
        return _items.Count;
    }

    public T this[int index]
    {
        get => _items[index]; // Get item at index
        set => _items[index] = value; // Set item at index
    }

}