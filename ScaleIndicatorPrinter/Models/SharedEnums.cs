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
        NetWeightAdjustment = 5,
        ColorName = 6
    }

    public enum MenuSelection
    {
        PrintLabel,
        Job,
        Operation,
        ViewPieceWeight,
        ViewNetWeightAdjustment,
        ViewBackgroundColor,
        ViewNetworkInfo,
        Reboot,
        AdjustPieceWeight,
        AdjustNetWeight,
        ChangeBackgroundColor,
        DisplayNetworkInfo,
        Rebooting
    }
    
}
