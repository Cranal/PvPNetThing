   M S B A M L   `   `   `  ����F  BPvPNetThing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:PvPNetThingPvPNetThing  5          PvPNetThing.BattleControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      +localclr-namespace:PvPNetThing     5      .��5      � $��5q�6     � ColumnDefinitions  5   
   �� 5      $
��Auto �6   �� 5      $	��210 �6   �� 5	      $	��100 �6   �� 5
      $	��210 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��50 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6   ��6-   ��  �� Path$ 
PlayerName��� 5   
   $��0��6   $��1��6   .+�5      �� $��25��6   $	��204��6$   
���   �$��2��6?   �� 5      $��21��6   
���  ��$��Center=�6P   $��2q�6k   ��6$   �� $ 	CurrentHp�6k   5      �� 5   
   $��1��6   $��1��6   $��Block��6-   '   !PvPNetThing.TargetSelectionContro  5   
   $��2��6&   $��1��63   $��Center=�6l    � DataContext 6C   �� $ BlockTarget�6l   �� 5#   
   $��0��6   $��3��6   .+�5$      �� $��25��6   $	��204��6$   
���   �$��2��6?   �� 5%      $��21��6   
���  ��$��Center=�6U   $��2q�6p   ��6$   �� $ EnemyCurrentHp�6p   5&      �� 5(   
   $��0��6   $��4��6   ��6-   �� $ 	EnemyName��� 5*   
   $��1��6   $��3��6   $��Attack��6-     5,   
   $��2��6&   $��3��63   $��Center=�6m    6C   �� $ AttackTarget�6m   �� 5.   
   $��3��6   $��0��6   $��2��6.   $��Attack��6B   $��25��6S   ��6_   �� $ AttackCommand��� 50   
   $��4��6   $��0��6    $��5��60   ��6D   �� $ 	BattleLog� �� NotifyOnSourceUpdated . 52      53      