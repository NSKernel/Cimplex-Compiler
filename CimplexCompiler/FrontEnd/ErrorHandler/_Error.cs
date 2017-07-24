using System;

namespace NanoStudio.Internel.Cimplex.Compiler.ErrorHandler
{
    public class Error
    {
        public ErrorID ID;
        public string ErrorDetail = "";
        public int LineNo;
    }

    public class ErrorHandler
    {
        public static void EhErrorOutput(Error Error)
        {
            string ErrorTitle = "";
            switch(Error.ID)
            {
                case ErrorID.C0001:
                    ErrorTitle = ErrorTable.C0001;
                    break;
                case ErrorID.C0002:
                    ErrorTitle = ErrorTable.C0002;
                    break;
                case ErrorID.C0003:
                    ErrorTitle = ErrorTable.C0003;
                    break;
                case ErrorID.C0004:
                    ErrorTitle = ErrorTable.C0004;
                    break;
                case ErrorID.C0005:
                    ErrorTitle = ErrorTable.C0005;
                    break;
                case ErrorID.C0006:
                    ErrorTitle = ErrorTable.C0006;
                    break;
                case ErrorID.C0007:
                    ErrorTitle = ErrorTable.C0007;
                    break;
                case ErrorID.C0008:
                    ErrorTitle = ErrorTable.C0008;
                    break;
                case ErrorID.C0009:
                    ErrorTitle = ErrorTable.C0009;
                    break;
                case ErrorID.C0010:
                    ErrorTitle = ErrorTable.C0010;
                    break;
                case ErrorID.C0011:
                    ErrorTitle = ErrorTable.C0011;
                    break;
                case ErrorID.C0012:
                    ErrorTitle = ErrorTable.C0012;
                    break;
                case ErrorID.C0013:
                    ErrorTitle = ErrorTable.C0013;
                    break;
            }
            Console.WriteLine("An error occoured.\n" + ErrorTitle + "\nError Detail: " + Error.ErrorDetail + "\nOccoured in line " + Error.LineNo.ToString());
        }
    }

    public enum ErrorID
    {
        C0001,
        C0002,
        C0003,
        C0004,
        C0005,
        C0006,
        C0007,
        C0008,
        C0009,
        C0010,
        C0011,
        C0012,
        C0013
    }
}
