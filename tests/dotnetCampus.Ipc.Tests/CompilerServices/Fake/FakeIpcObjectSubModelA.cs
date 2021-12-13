namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal class FakeIpcObjectSubModelA
    {
        public FakeIpcObjectSubModelA()
        {
        }

        public FakeIpcObjectSubModelA(double a, uint b, int c, byte d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public double A { get; set; }
        public uint B { get; set; }
        public int C { get; set; }
        public byte D { get; set; }
    }
}
