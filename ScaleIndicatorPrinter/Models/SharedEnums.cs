using System;
using Microsoft.SPOT;

namespace ScaleIndicatorPrinter.Models
{
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
        PieceWeight = 4,
        NetWeightAdjustment = 5
    }

    public enum MenuSelection
    {
        PrintLabel,
        Job,
        Operation,
        ViewPieceWeight,
        ViewNetWeightAdjustment,
        AdjustPieceWeight,
        AdjustNetWeight
    }
    
}
