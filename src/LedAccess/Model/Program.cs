namespace LedAccess.Model
{
    using System;
    using System.Collections.Generic;

    public class Program
    {
        #region properties
        public Parameter FirstParameter { get; }
        public Parameter SecondParameter { get; }
        public Parameter ThirdParameter { get; }
        public string ProgramName { get; }
        public string ProgramTooltip { get; }
        public int CommandDecoding { get; }
        #endregion properties

        #region constructors
        public Program(String programName, String programTooltip, Parameter p1, Parameter p2, Parameter p3, int commandDecoding)
        {
            this.ProgramName = programName;
            this.ProgramTooltip = programTooltip;
            this.FirstParameter = p1;
            this.SecondParameter = p2;
            this.ThirdParameter = p3;
            this.CommandDecoding = commandDecoding;
        }
        #endregion constructors

        #region functions
        public byte[] GetByteEncoding()
        {
            List<byte> encoding = new List<byte>();
            encoding.Add((byte)2);
            encoding.Add((byte)CommandDecoding);

            if (FirstParameter.Available)
            {
                encoding.Add((byte)FirstParameter.Value);
            }
            if (SecondParameter.Available)
            {
                encoding.Add((byte)SecondParameter.Value);
            }
            if (ThirdParameter.Available)
            {
                encoding.Add((byte)ThirdParameter.Value);
            }

            return encoding.ToArray();
        }

        public override string ToString()
        {
            return $"{CommandDecoding}: {ProgramName}";
        }
        #endregion functions
    }

    public class Parameter
    {
        #region constructors
        public Parameter(String name, int min, int max)
        {
            ParameterName = name;
            MinValue = min;
            MaxValue = max;
            Available = true;
        }

        public Parameter()
        {
            //default constructor vor unused parameters
            Available = false;
        }
        #endregion constructors

        #region properties
        public string ParameterName { get; }
        public int MinValue { get; }
        public int MaxValue { get; }
        public int Value { get; set; }
        public bool Available { get; }
        #endregion properties
    }
}
