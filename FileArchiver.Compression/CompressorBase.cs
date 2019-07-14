using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace FileArchiver.Compression
{
	public abstract class CompressorBase
	{
		private static readonly int Threads = Environment.ProcessorCount;
		protected int BlockSize;
		private bool _cancelled;
		private ConcurrentQueue<Exception> _exceptions = new ConcurrentQueue<Exception>();
		private readonly ManualResetEvent _saveEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _readEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent[] _processEvents = new ManualResetEvent[Threads];
		private readonly BlockManager _reader = new BlockManager();
		private readonly BlockManager _writer = new BlockManager();

		protected abstract Block ReadBlock(Stream stream, int counter);
		protected abstract void WriteBlock(Stream stream, Block block);
		protected abstract void ProcessBlock(Block block);

		protected CompressorBase(int blockSize)
		{
			BlockSize = blockSize;
		}

		public void Do(string inputFile, string outputFile)
		{
			_exceptions = new ConcurrentQueue<Exception>();
			using (var readFileStream = new FileStream(inputFile, FileMode.Open))
			using (var writeFileStream = new FileStream(outputFile, FileMode.Create))
			{
				var reader = new Thread(() => Read(readFileStream));
				reader.Start();

				for (int i = 0; i < _processEvents.Length; i++)
				{
					_processEvents[i] = new ManualResetEvent(false);
					var eventIndex = i;
					var thread = new Thread(() => ProcessObject(eventIndex));
					thread.Start();
				}

				var writer = new Thread(() => Write(writeFileStream));
				writer.Start();
				WaitHandle.WaitAll(_processEvents);
				_writer.Stop();
				WaitHandle.WaitAll(new WaitHandle[] { _saveEvent, _readEvent });
			}
			if (_exceptions.Count > 0)
				throw new AggregateException(_exceptions);
		}

		private void Write(Stream stream)
		{
			try
			{
				Block block = _writer.Dequeue();
				while (block != null && !_cancelled)
				{
					WriteBlock(stream, block);
					block = _writer.Dequeue();
				}
				_writer.Stop();
			}
			catch (Exception e)
			{
				_cancelled = true;
				_exceptions.Enqueue(e);
			}
			finally
			{
				_saveEvent.Set();
			}
		}

		private void Read(Stream stream)
		{
			try
			{
				int counter = 0;
				while (stream.Position < stream.Length && !_cancelled)
				{
					var block = ReadBlock(stream, counter);
					_reader.Enqueue(block);
					counter++;
				}
				_reader.Stop();
			}
			catch (Exception e)
			{
				_cancelled = true;
				_exceptions.Enqueue(e);
			}
			finally
			{
				_readEvent.Set();
			}
		}

		private void ProcessObject(object i)
		{
			try
			{
				Block block = _reader.Dequeue();
				while (block != null && !_cancelled)
				{
					ProcessBlock(block);
					_writer.Enqueue(block);
					block = _reader.Dequeue();
				}
			}
			catch (Exception e)
			{
				_cancelled = true;
				_exceptions.Enqueue(e);
			}
			finally
			{
				_processEvents[(int)i].Set();
			}
		}
	}
}