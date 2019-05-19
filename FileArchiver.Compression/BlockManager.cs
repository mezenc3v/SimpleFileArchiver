using System;
using System.Collections.Generic;
using System.Threading;

namespace FileArchiver.Compression
{
	public class BlockManager
	{
		private readonly object _locker = new object();
		private readonly Queue<Block> _queue = new Queue<Block>();
		private bool _stopped;
		private int _blockId;

		public void Enqueue(Block block)
		{
			lock (_locker)
			{
				if (_stopped)
					throw new InvalidOperationException("Queue already stopped");

				while (block.Id != _blockId)
				{
					Monitor.Wait(_locker);
				}

				var newBlock = new Block(_blockId, block.Buffer, block.CompressedBuffer);
				_queue.Enqueue(newBlock);
				_blockId++;
				Monitor.PulseAll(_locker);
			}
		}

		public Block Dequeue()
		{
			lock (_locker)
			{
				while (_queue.Count == 0 && !_stopped)
					Monitor.Wait(_locker);

				return _queue.Count == 0
					? null :
					_queue.Dequeue();
			}
		}

		public void Stop()
		{
			lock (_locker)
			{
				_stopped = true;
				Monitor.PulseAll(_locker);
			}
		}
	}
}