// ReSharper disable InconsistentNaming
namespace SixLabors.ImageSharp.Benchmarks.ColorSpaces.Bulk
{
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;

    using SixLabors.Memory;
    using SixLabors.ImageSharp.PixelFormats;

    [Config(typeof(Config.ShortClr))]
    public abstract class PackFromVector4<TPixel>
        where TPixel : struct, IPixel<TPixel>
    {
        private IBuffer<Vector4> source;

        private IBuffer<TPixel> destination;

        [Params(16, 128, 512)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.destination = Configuration.Default.MemoryAllocator.Allocate<TPixel>(this.Count);
            this.source = Configuration.Default.MemoryAllocator.Allocate<Vector4>(this.Count);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.destination.Dispose();
            this.source.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void PerElement()
        {
            ref Vector4 s = ref MemoryMarshal.GetReference(this.source.GetSpan());
            ref TPixel d = ref MemoryMarshal.GetReference(this.destination.GetSpan());
            
            for (int i = 0; i < this.Count; i++)
            {
                Unsafe.Add(ref d, i).PackFromVector4(Unsafe.Add(ref s, i));
            }
        }

        [Benchmark]
        public void CommonBulk()
        {
            new PixelOperations<TPixel>().PackFromVector4(this.source.GetSpan(), this.destination.GetSpan(), this.Count);
        }

        [Benchmark]
        public void OptimizedBulk()
        {
            PixelOperations<TPixel>.Instance.PackFromVector4(this.source.GetSpan(), this.destination.GetSpan(), this.Count);
        }
    }

    public class PackFromVector4_Rgba32 : PackFromVector4<Rgba32>
    {

    }
}