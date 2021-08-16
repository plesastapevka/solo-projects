using System;

namespace steganography
{
    class HuffmanDecompress
	{
		private const int MAX_TREE_NODES = 511;

		public class BitStream
		{
			public byte[] BytePointer;
			public uint BitPosition;
			public uint Index;
		}

		public struct Symbol
		{
			public int Sym;
			public uint Count;
			public uint Code;
			public uint Bits;
		}

		public class DecodeNode
		{
			public DecodeNode ChildA;
			public DecodeNode ChildB;
			public int Symbol;
		}

		private static void InitBitstream(ref BitStream stream, byte[] buffer)
		{
			stream.BytePointer = buffer;
			stream.BitPosition = 0;
		}

		private static uint ReadBit(ref BitStream stream)
		{
			byte[] buffer = stream.BytePointer;
			uint bit = stream.BitPosition;

			uint x = (uint)(Convert.ToBoolean((buffer[stream.Index] & (1 << (int)(7 - bit)))) ? 1 : 0);
			bit = (bit + 1) & 7;

			if (!Convert.ToBoolean(bit))
			{
				++stream.Index;
			}

			stream.BitPosition = bit;

			return x;
		}

		private static uint Read8Bits(ref BitStream stream)
		{
			byte[] buffer = stream.BytePointer;
			uint bit = stream.BitPosition;
			uint x = (uint)((buffer[stream.Index] << (int)bit) | (buffer[stream.Index + 1] >> (int)(8 - bit)));
			++stream.Index;

			return x;
		}

		private static DecodeNode RecoverTree(DecodeNode[] nodes, ref BitStream stream, ref uint nodenum)
		{
			var thisNode = nodes[nodenum];
			nodenum = nodenum + 1;
			thisNode.Symbol = -1;
			thisNode.ChildA = null;
			thisNode.ChildB = null;

			if (Convert.ToBoolean(ReadBit(ref stream)))
			{
				thisNode.Symbol = (int)Read8Bits(ref stream);
				return thisNode;
			}

			thisNode.ChildA = RecoverTree(nodes, ref stream, ref nodenum);
			thisNode.ChildB = RecoverTree(nodes, ref stream, ref nodenum);

			return thisNode;
		}

		public static void Decompress(byte[] input, byte[] output, uint inputSize, uint outputSize)
		{
			DecodeNode[] nodes = new DecodeNode[MAX_TREE_NODES];

			for (int counter = 0; counter < nodes.Length; ++counter)
			{
				nodes[counter] = new DecodeNode();
			}

			BitStream stream = new BitStream();
			uint i;

			if (inputSize < 1)
				return;

			InitBitstream(ref stream, input);
			uint nodeCount = 0;
			var root = RecoverTree(nodes, ref stream, ref nodeCount);
			var buffer = output;

			for (i = 0; i < outputSize; ++i)
			{
				var node = root;

				while (node.Symbol < 0)
				{
					node = Convert.ToBoolean(ReadBit(ref stream)) ? node.ChildB : node.ChildA;
				}

				buffer[i] = (byte)node.Symbol;
			}
		}
	}
}