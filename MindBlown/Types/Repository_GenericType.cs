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

    // Will need to check in future if these methods are needed, if not delete it in future.

    // public IEnumerable<T> Where(Func<T, bool> predicate)
    // {
    //     return _items.Where(predicate).ToList(); // or just return _items.Where(predicate) for lazy evaluation
    // }

    // public T? FirstOrDefault(Func<T, bool> predicate)
    // {
    //     return _items.FirstOrDefault(predicate);
    // }

    // public IEnumerable<T> Union(IEnumerable<T> other)
    // {
    //     return _items.Union(other);
    // }

    // public bool Any()
    // {
    //     return _items.Any();
    // }

    // public bool Any(Func<T, bool> predicate)
    // {
    //     return _items.Any(predicate);
    // }

    // public List<T> ToList()
    // {
    //     return _items.ToList();
    // }

    // public IEnumerator<T> GetEnumerator()
    // {
    //     return _items.GetEnumerator();
    // }
}