using System;
using Microsoft.SPOT;

namespace ScaleIndicatorPrinter.Models
{
    public enum DataBits
    {
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9
    }

    public enum RecievedData
    {
        ScaleIndicator,
        ScannerJobAndSuffix,
        ScannerOperation,
        None
    }

    public enum InformationType
    {
        LabelFormat = 0,
        JobNumber = 1,
        OperationNumber = 2,
        ShopTrakTransactionsURL = 3,
        PieceWeight
    }

    public enum MenuSelection
    {
        PrintLabel,
        Job,
        Operation,
        ViewPieceWeight,
        AdjustPieceWeight
    }

}
