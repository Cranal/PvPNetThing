   M S B A M L   `   `   `  ����F  BPvPNetThing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:PvPNetThingPvPNetThing  5          PvPNetThing.LoginControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +localclr-namespace:PvPNetThing     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��Login��6   $��0��6!   $��0��6.   $��5q�6>   �� 5   
   $	��200��6   $��0��6   $��1��6+   ��6;   ��  �� Path$ Login� �� NotifyOnTargetUpdated .  �� UpdateSourceTrigger$ PropertyChanged=��� 5   
   $��Password��6   $��1��6$   $��0��61   $��5q�6A   �� 5   
   $	��200��6   $��1��6   $��1��6+   ��6;   �� $ Password� . $ PropertyChanged=��� 5   
   $��2��6   $��0��6"   $��2��62    �� Orientation$ 
Horizontal=�6F   $��Center=�6_   .+�5      ��  �� 	IsDefault . $��5q�6&   $	��100��61   $��25��6=   $��Enter��6I   ��6Y   �� $ ClickCommand���6   �� �� 5      $	��100��6   $��5q�6!   $��25��6,   $��Create��68   ��6I   �� $ CreateCommand���6p   �� 5      �� 5   
   $��3��6   $��0��6!   $��2��61   $0��*Error occured during the login. Try again.��6E   y�6w   �� $ IsLoginError�5!      5"      