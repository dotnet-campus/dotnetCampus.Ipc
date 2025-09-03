namespace IpcRemotingObjectServerDemo;

class Foo : IFoo
{
    public string Name { get; set; } = "Foo";

    public int Add(int a, int b)
    {
        Console.WriteLine($"a({a})+b({b})={a + b}");
        return a + b;
    }

    public async Task<string> AddAsync(string a, int b)
    {
        return await Task.Run(() =>
        {
            Console.WriteLine($"a({a})+b({b})={a + b}");
            return a + b;
        });
    }
}
