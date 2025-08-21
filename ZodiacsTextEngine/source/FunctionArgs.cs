using System;

namespace ZodiacsTextEngine
{
	public readonly struct FunctionArgs
	{
		public readonly string[] args;
		public readonly string raw;

		public FunctionArgs(string input)
		{
			raw = input;
			args = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public FunctionArgs(string[] args, string raw)
		{
			this.args = args;
			this.raw = raw;
		}

		public string this[int index]
		{
			get
			{
				if(index < 0 || index >= args.Length)
				{
					return "";
				}
				return args[index];
			}
		}

		public int Length => args.Length;

		public int GetInt(int index)
		{
			return int.Parse(this[index]);
		}

		public bool TryGetInt(int index, out int value)
		{
			if(index < 0 || index >= args.Length)
			{
				value = 0;
				return false;
			}
			return int.TryParse(args[index], out value);
		}

		public float GetFloat(int index)
		{
			return float.Parse(this[index]);
		}

		public bool TryGetFloat(int index, out float value)
		{
			if(index < 0 || index >= args.Length)
			{
				value = 0f;
				return false;
			}
			return float.TryParse(args[index], out value);
		}

		public string GetString(int index)
		{
			return this[index];
		}

		public bool TryGetString(int index, out string value)
		{
			if(index < 0 || index >= args.Length)
			{
				value = null;
				return false;
			}
			value = this[index];
			return true;
		}
	}
}