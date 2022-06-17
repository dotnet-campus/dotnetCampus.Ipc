namespace IpcRemotingObjectServerDemo;

class Foo : IFoo
{
    public int Add(int a, int b)
    {
        Console.WriteLine($"a({a})+b({b})={a + b}");
        return a + b;
    }
}
