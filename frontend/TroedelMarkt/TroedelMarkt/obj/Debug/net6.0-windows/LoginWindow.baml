   M S B A M L   `   `   `  ????G  CTroedelMarkt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:TroedelMarktTroedelMarkt  5   	       TroedelMarkt.LoginWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:TroedelMarkt     5   	     ? Title$   Trödelmarkt Manager Login??5   	   $	??360??6+   $	??500??68    ? WindowStyle$ 
ToolWindow=?6D    ? 
ResizeMode$ NoResize=?6]    ? Topmost$
 True??6s   .??5	      ?  ? ColumnDefinitions 5
   
   ?? 5      ?? 5       ? RowDefinitions 5   
   ?? 5      ?? 5      ?? 5      .+?5   
   ?? .+?5      ?? $??Datenbank Adresse??6   $??Center=?60   $??10,10,10,10q?6R   ?? 5      -   5         ??Name$ 
TBoxAdress??5       ?? TextWrapping$
 Wrap=?6*   $??10,10,10,10q?6>   .??6S   ??5      ?? 5       ?? Path$ Adress?6   	 ?? UpdateSourceTrigger$	 PropertyChanged=?6w   
 ?? RelativeSource
 6-   ??  ?? AncestorType   TroedelMarkt.LoginWindow"   ?? ValidationRules 5      %  TroedelMarkt.TraderIDValidation  5         pattern$b \^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)[.]){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$??67   5      .??5       5      5      ?? 5   
   $??1??6   .+?5       ?? $
??Port??6   $??10,10,10,10q?6#   ?? 5!      -   5        $ TBoxPort??5!      $
 Wrap=?6(   $??10,10,10,10q?6<   .??6Q   ??5"      ?? 5#      $
 Port?6   $	 PropertyChanged=?6u   
 6+   ?? "   5$      !  TroedelMarkt.PortValidation  5%      5'      .??5(       5)      5*      ?? 5+   
   $??1??6   $??2??6"   .+?5,      ?? $??Passwort??6   $??10,10,10,10q?6'   $??Center=?6<   G? 5-      -   5         G?Name$ PBoxPassword??5-      $??10,10,10,10q?60    G? Password$  ??6E   5.      ?? 5/   
   $??2??6   $??2??6"   .+?50      ?? -   5         ??Name$ TbnLogin??50      $??Anmelden??6'   $??10,10,10,10q?6:    ?? 	IsDefault$
 True??6f   ?? 51      -   5         ??Name$ TBlockResponce??51      $
??Wrap=?60   $??10,10,10,10q?6D    ?? TextAlignment$ Center=?6Y   52      53      54      