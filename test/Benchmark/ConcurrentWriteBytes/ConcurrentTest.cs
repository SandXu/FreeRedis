﻿using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentWriteBytes
{
    [MemoryDiagnoser, CoreJob, MarkdownExporter, RPlotExporter]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class ConcurrentTest
    {
        public readonly NewConcurrentQueue<int> _newQueue;
        public readonly SourceConcurrentQueue<int> _sourceQueue;
        public readonly ConcurrentQueue<int> _queue;
        public const int Count = 1000;

        public ConcurrentTest()
        {
            _newQueue = new NewConcurrentQueue<int>(default);
            _queue = new ConcurrentQueue<int>();
            _sourceQueue = new SourceConcurrentQueue<int>();
        }

        [Benchmark]
        public void NewConcurrentQueue()
        {
            Parallel.For(0, Count, (i) => {
                _newQueue.Enqueue(i);
            });
            Parallel.For(0, Count, (i) => {
                var _ = _newQueue.Dequeue();
            });
        }

        [Benchmark]
        public void SourceConcurrentQueue()
        {
            Parallel.For(0, Count, (i) => {
                _sourceQueue.Enqueue(i);
            });
            Parallel.For(0, Count, (i) => {
                _sourceQueue.TryDequeue(out var _);
            });
        }

        [Benchmark]
        public void ConcurrentQueue()
        {
            Parallel.For(0, Count, (i) => {
                _queue.Enqueue(i);
            });
            Parallel.For(0, Count, (i) => {
                _queue.TryDequeue(out var _);
            });
        }

    }

}
