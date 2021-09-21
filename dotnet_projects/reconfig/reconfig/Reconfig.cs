using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace reconfig
{
	internal class Reconfig
	{
		private readonly string _file;
		private readonly List<int> _nums;
		private bool _orientation = true;
		public Reconfig(string file)
		{
			_file = file;
			_nums = new List<int>();
		}

		private void ToFile()
		{
			using StreamWriter file = new("OUT.txt");
			foreach (var item in _nums)
			{
				file.Write(item + " ");
			}
		}

		public void Read()
		{
			string line;
			var f = new StreamReader(_file);
			while ((line = f.ReadLine()) != null)
			{
				_nums.Add(int.Parse(line));
			}
		}

		private void Pad()
		{
			_nums.Add(_nums.Count + 1);
			_nums.Insert(0, 0);
		}

		private void PrintArr()
		{
			foreach (var t in _nums)
			{
				if (t == -1) continue;
				Console.Write(t + " ");
			}

			Console.WriteLine("\n");
		}

		private static bool IsSorted(IReadOnlyList<int> a)
		{
			var i = a.Count - 1;
			if (i <= 0) return true;
			if ((i & 1) > 0) { if (a[i] < a[i - 1]) return false; i--; }
			for (var ai = a[i]; i > 0; i -= 2)
				if (ai < (ai = a[i - 1]) || ai < (ai = a[i - 2])) return false;
			return a[0] <= a[1];
		}

		private void ChOrientation()
		{
			_orientation = !_orientation;
		}
		// SIMPLE REVERSAL SORT
		public void SimpleReversalSort(bool verbose)
		{
			var step = 0;
			var clock = new Stopwatch();
			clock.Start();
			for (var i = 0; i < _nums.Count - 1; i++)
			{
				var j = FindPositionOf(i + 1);
				if (j != i)
				{
					step++;
					_nums.Reverse(i, j - i + 1);
					if (verbose)
					{
						PrintArr();
					}
				}
				if (IsSorted(_nums))
				{
					break;
				}

			}
			clock.Stop();
			if (verbose)
			{
				PrintArr();
			}

			if (IsSorted(_nums))
			{
				Console.WriteLine("Sorted!");
				ToFile();
			}
			else
			{
				Console.WriteLine("Error!");
			}

			var ts = clock.Elapsed;
			Console.WriteLine("Finished in: " + ts.TotalMilliseconds + "ms");
			Console.WriteLine("Steps: " + step);
		}

		private int FindPositionOf(int s)
		{
			var res = -1;
			for (var i = 0; i < _nums.Count; i++)
			{
				if (_nums[i] == s)
				{
					res = i;
				}
			}
			return res;
		}
		
		// IMPROVED SORT
		public void SortImprovedBreakpoint(bool verbose)
		{
			var step = 0;
			Pad();
			var clock = new Stopwatch();
			clock.Start();
			if (verbose)
			{
				PrintArr();
			}
			while (HasBp(_nums))
			{
				List<(int, int)> increasing;
				List<(int, int)> decreasing;
				(increasing, decreasing) = GetIntervals();
				var reversal = decreasing.Count > 0 ? ChooseReversal(decreasing) : increasing[0];
				
				var start = reversal.Item1 + 1;
				var end = reversal.Item2 - reversal.Item1 - 1;
				_nums.Reverse(start, end);
				
				if (verbose)
				{
					PrintArr();
				}
				step++;
			}
			step++;
			if (verbose)
			{
				PrintArr();
			}
			clock.Stop();
			if (IsSorted(_nums))
			{
				Console.WriteLine("Sorted!");
				ToFile();
			}
			else
			{
				Console.WriteLine("Error!");
			}

			var ts = clock.Elapsed;
			Console.WriteLine("Finished in: " + ts.TotalMilliseconds + "ms");
			Console.WriteLine("Steps: " + step);
		}


		private static bool HasBp(List<int> seq)
		{
			for (var i = 1; i < seq.Count; i++)
			{
				if (seq[i] != seq[i - 1] + 1)
				{
					return true;
				}
			}
			return false;
		}

		private (List<(int, int)>, List<(int, int)>) GetIntervals()
		{
			var d = new List<int>();
			var increasing = new List<(int, int)>();
			var decreasing = new List<(int, int)>();
			var start = 0;

			for (var i = 0; i < _nums.Count - 1; i++)
			{
				d.Add(_nums[i + 1] - _nums[i]);
			}

			for (var i = 0; i < d.Count; i++)
			{
				if ((Math.Abs(d[i]) == 1) && (d[i] == d[start])) continue;
				if (start > 0)
				{
					if (d[start] == 1) increasing.Add((start, i + 1));
					else decreasing.Add((start, i + 1));
				}
				start = i + 1;
			}
			return (increasing, decreasing);
		}

		private (int, int) ChooseReversal(List<(int, int)> dec)
		{
			(int, int) min = (int.MaxValue, -1);
			(int, int) minS = (int.MaxValue, -1);
			for (var i = 0; i < dec.Count; i++)
			{
				var item = dec[i];
				if (_nums[item.Item2 - 1] >= min.Item1) continue;
				if (item.Item1 - item.Item2 == -1)
				{
					if (_nums[item.Item2 - 1] < minS.Item1)
					{
						minS = (_nums[item.Item2 - 1], i);
					}
				}
				else
				{
					min = (_nums[item.Item2 - 1], i);
				}
			}
			int start;
			int end;
			if (min.Item2 == -1)
			{
				start = _nums.IndexOf(minS.Item1 - 1);
				end = dec[minS.Item2].Item2;
			}
			else
			{
				start = _nums.IndexOf(min.Item1 - 1);
				end = dec[min.Item2].Item2;
			}

			return start > end ? (end - 1, start + 1) : (start, end);
		}
		
		public void CustomSort(bool verbose)
		{
			var step = 0;
			Pad();
			var clock = new Stopwatch();
			clock.Start();
			if (verbose)
			{
				PrintArr();
			}

			if (_nums.Count % 2 != 0)
			{
				_nums.Add(_nums.Count);
			}
			
			var one = false;
			var two = false;
			var first = _nums.Count / 2 - 1;
			var second = first + 1;
			var ifirst = -1;
			var isecond = -1;
			for (var i = 0; i < _nums.Count; i++)
			{
				if (_nums[i] == first)
				{
					ifirst = i;
					one = true;
					if (two) break;
				}
				else if (_nums[i] == second)
				{
					isecond = i;
					two = true;
					if (one) break;
				}
			}
			var temp = _nums[first];
			_nums[first] = _nums[ifirst];
			_nums[ifirst] = temp;

			temp = _nums[second];
			_nums[second] = _nums[isecond];
			_nums[isecond] = temp;
			(int, int) center = (first, second + 1);

			while (HasBp(_nums))
			{
				var fLeft = -1;
				var sLeft = -1;
				for (var i = 0; i < center.Item1; i++)
				{
					if (_orientation)
					{
						if (_nums[i] == _nums[center.Item1] - 1)
						{
							fLeft = i;
						}
						else if (_nums[i] == _nums[center.Item2 - 1] + 1)
						{
							sLeft = i;
						}
					}
					else
					{
						if (_nums[i] == _nums[center.Item1] + 1)
						{
							fLeft = i;
						}
						else if (_nums[i] == _nums[center.Item2 - 1] - 1)
						{
							sLeft = i;
						}
					}
				}
				var fRight = -1;
				var sRight = -1;
				for (var i = center.Item2; i < _nums.Count; i++)
				{
					if (_orientation)
					{
						if (_nums[i] == _nums[center.Item1] - 1)
						{
							fRight = i;
						}
						else if (_nums[i] == _nums[center.Item2 - 1] + 1)
						{
							sRight = i;
						}
					}
					else
					{
						if (_nums[i] == _nums[center.Item1] + 1)
						{
							fRight = i;
						}
						else if (_nums[i] == _nums[center.Item2 - 1] - 1)
						{
							sRight = i;
						}
					}
				}

				if (sLeft != -1 && fRight != -1)
				{
					_nums.Reverse(center.Item1, center.Item2 - center.Item1);
					_nums.Reverse(sLeft, center.Item1 - sLeft);
					_nums.Reverse(center.Item2, fRight - center.Item2 + 1);
					step += 3;
					center = NewCenter(center);
					ChOrientation();
				}
				else if (fLeft != -1 && sRight != -1)
				{
					_nums.Reverse(fLeft, center.Item1 - fLeft);
					_nums.Reverse(center.Item2, sRight - center.Item2 + 1);
					step += 2;
					center = NewCenter(center);
				}
				else if (fRight != -1 && sRight != -1)
				{
					if (fRight < sRight)
					{
						int tmp = fRight;
						fRight = sRight;
						sRight = tmp;
						_nums.Reverse(center.Item1, center.Item2 - center.Item1);
						step++;
						ChOrientation();
					}
					_nums.Reverse(center.Item2, sRight - center.Item2 + 1);
					center = NewCenterRight(center);
					_nums.Reverse(center.Item1, center.Item2 - center.Item1);
					_nums.Reverse(center.Item2, fRight - center.Item2 + 1);
					step += 2;
					center = NewCenterRight(center);
					ChOrientation();
				}
				else if (fLeft != -1 && sLeft != -1)
				{
					if(fLeft < sLeft)
					{
						var tmp = fLeft;
						fLeft = sLeft;
						sLeft = tmp;
						_nums.Reverse(center.Item1, center.Item2 - center.Item1);
						step++;
						ChOrientation();
					}
					_nums.Reverse(fLeft, center.Item1 - fLeft);
					center = NewCenterLeft(center);
					_nums.Reverse(center.Item1, center.Item2 - center.Item1);
					_nums.Reverse(sLeft, center.Item1 - sLeft);
					center = NewCenterLeft(center);
					step += 3;
					ChOrientation();
				}
				else
				{
					int flip;
					var left = false;
					if(fLeft != -1)  // left
					{
						left = true;
						flip = fLeft;
					}
					else if(sLeft != -1)  // left
					{
						left = true;
						flip = sLeft;
					}
					else if(fRight != -1)  // right
					{
						flip = fRight;
					}
					else if(sRight != -1)
					{
						flip = sRight;
					}
					else
					{
						Console.WriteLine("Error!");
						return;
					}
					
					if (left)
					{
						if (Math.Abs(_nums[flip] - _nums[center.Item1]) == 1)
						{
							_nums.Reverse(flip, center.Item1 - flip);
							center = NewCenterLeft(center);
							step++;
						}
						else
						{
							_nums.Reverse(center.Item1, center.Item2 - center.Item1);
							ChOrientation();
							_nums.Reverse(flip, center.Item1 - flip);
							center = NewCenterLeft(center);
							step += 2 ;
						}
					}
					else
					{
						if (Math.Abs(_nums[flip] - _nums[center.Item1]) == 1)
						{
							_nums.Reverse(center.Item1, center.Item2 - center.Item1);
							ChOrientation();
							_nums.Reverse(center.Item2, Math.Abs(center.Item2 - flip - 1));
							step += 2;
							center = NewCenterRight(center);
						}
						else
						{
							_nums.Reverse(center.Item2, Math.Abs(center.Item2 - flip - 1));
							step++;
							center = NewCenterRight(center);
						}
					}
				}
				if (verbose)
				{
					PrintArr();
				}
			}

			clock.Stop();
			if (verbose)
			{
				PrintArr();
			}

			if (_nums.Count % 2 != 0)
			{
				_nums.RemoveAt(_nums.Count - 1);
			}

			if (IsSorted(_nums))
			{
				Console.WriteLine("Sorted!");
				ToFile();
			}
			else
			{
				Console.WriteLine("Error!");
			}
			var ts = clock.Elapsed;
			Console.WriteLine("Finished in: " + ts.TotalMilliseconds + "ms");

			Console.WriteLine("Steps: " + step);
		}

		private (int, int) NewCenter((int, int) old)
		{
			var low = old.Item1;
			var high = old.Item2;
			for (var i = old.Item1; i > 0; i--)
			{
				if (Math.Abs(_nums[i] - _nums[i - 1]) == 1)
				{
					low = i - 1;
				}
				else break;
			}
			for (var i = old.Item2 - 1; i < _nums.Count - 1; i++)
			{
				if (Math.Abs(_nums[i] - _nums[i + 1]) == 1)
				{
					high = i + 1;
				}
				else break;
			}
			return (low, high + 1);
		}

		private (int, int) NewCenterRight((int, int) old)
		{
			var low = old.Item1;
			var high = old.Item2;
			for (var i = old.Item2 - 1; i < _nums.Count - 1; i++)
			{
				if (Math.Abs(_nums[i] - _nums[i + 1]) == 1)
				{
					high = i + 1;
				}
				else break;
			}
			return (low, high + 1);
		}

		private (int, int) NewCenterLeft((int, int) old)
		{
			var low = old.Item1;
			var high = old.Item2;
			for (var i = old.Item1; i > 0; i--)
			{
				if (Math.Abs(_nums[i] - _nums[i - 1]) == 1)
				{
					low = i - 1;
				}
				else break;
			}
			return (low, high);
		}
	}
}