   M S B A M L   `   `   `  ????G  CTroedelMarkt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TroedelMarktTroedelMarkt  5   	       TroedelMarkt.Window1   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TroedelMarkt     5   	   -   5          ? Title$*  $Trödelmarkt Manager Händleransicht??5   	   $	??450??64   $	??800??6A   .??5	      ?  ? RowDefinitions 5
   
   ?? 5      ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      ?? 5      .+?5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ DGTrader??5         AutoGenerateColumns$ False??6?   $??2??6?    ?? AlternationCount$ 2??6     AlternatingRowBackground
 ???w???6P   ?? *	Traders+ ?? RelativeSource ?? *FindAncestor+ ?? AncestorType   TroedelMarkt.Window1"  	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 Händler ID??6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   ?? *
TraderID+ ?? Mode$ OneWay=?  5      $

 Name??6%    63   ?? *Name+$ TwoWay=?  5      $
 Bilanz??6%    65   ?? *	Balance+ ?? StringFormat$ C?? ?? ConverterCulture$ de-De??$ OneWay=?  5      $
 Gewinn??6%    65   ?? *	Revenue+$ C??$ de-De??$ OneWay=?  5      $
 Provisionsrate in %??6%    6B   ?? *ProvisionRatePerc+$ TwoWay=?$ {0:N}??  5      $
 	Provision??6%    68   ?? *	Provision+$ C??$ de-De??$ OneWay=?5      ?? 5   
   $??1??6   $??1??6%    ?? Orientation$ 
Horizontal=?62   $??Right=?6K   .+?5      ?? -   5         ??Name$ 	ExportCSV??5      $??Daten exportieren??6(   $??Center=?6D   $??Bottom=?6a   $??10,10,10,10q?6|   6?   ?? 5      $??Bottom=?6   .+?5      ?? $??Händler ID??6   $??Stretch=?6-   $??Stretch=?6R   $??10,10,10,10q?6u   ?? 5       -   5         ??Name$ TBoxTraderID??5        ?? TextWrapping$
 Wrap=?60   $??Bottom=?6D   $??10,10,10,10q?6_   .??6t   ??5!      ?? 5"       ?? Path$ newTraderID?6"    ?? UpdateSourceTrigger$ PropertyChanged=?6|    66   ?? "   ?? ValidationRules 5#      %  TroedelMarkt.TraderIDValidation  5$   "      pattern$ ^[a-zA-Z0-9]*$??6;   5&      .??5'   $   5(      ?? 5)      -   5        $ 	BtnAddTdr??5)      $??Händler hinzufügen??6,   $??10,10,10,10q?6I   $??Center=?6^   $??Bottom=?6{   6?   5*      ?? 5+      -   5        $ 	BtnDelTdr??5+      $??Händler löschen??6(   $??10,10,10,10q?6B   $??Bottom=?6W   6r   5,      ? 5-   
   $??1??6    5.      ?? 5/      ?? 50       52      ?? 53      ?? 54      .+?56      ?? -   5         ??Name$ 
TBlockSumm??56      $??10,10,10,10q?6,   $
??Wrap=?6A   $??Summe??6U   ?? 57      -   5        $ TBlockProvSumm??57      $??1??60   $??10,10,10,10q?6@   $
??Wrap=?6U   $??
Prov Summe??6i   ?? 58      $??1??6   $??Bottom=?6&   .+?59      ?? -	   5        $ BtnUpdateData??59      $??Daten aktualisieren??60   $??10,10,10,10q?6N   $??Center=?6c   6~   5:      ?? 5;      -
   5        $ BtnUpdateTraders??5;      $??1??6/   $'??!Makierte Händler synchronisieren??6?   $??10,10,10,10q?6j   $??1??6   $??Bottom=?6?   ${?False??6?   ? 5<      ??6   ?? *IsMouseOver+ ?? ElementName$ BtnUpdateTraders??$ OneWay=? ? PlacementTarget 6^   ?? $ BtnUpdateTraders??.??5=      ?? $3??-Bearbeitete Händler werden in grün markiert??6    ?? Padding$ 10,10,10,10q?6O   
???????6e   5>      5?      5@      5A      