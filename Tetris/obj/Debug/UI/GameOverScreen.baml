   M S B A M L   `   `   `  ����A  =Tetris, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:TetrisTetris  5          Tetris.GameOverScreen   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      &localclr-namespace:Tetris     5      $	��300��5      $	��300��6     � 	Resources  5	       �� Source�� 5
   
   $ MyStyles.xaml?�6   %    .��5      �  � RowDefinitions 5   
   �� 5      �� 5      ��5   
   �� 5       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W    � SpreadMethod$ Reflect=�6j   .��5      �  � Color$ Yellow��6    � Offset$ 1Q�6.   � 5      $ 
DarkOrange��6   5      .+�5   
   � $��1��6   	 � ColumnDefinitions	 5      �� 5      �� 5      .+�5      �� -   5        $��RestartButton��5      $��0��6*   $��20,30,20,30q�6:   $��Restart��6O   �� 5      -   5        $��
ExitButton��5      $��1��6'   $��20,30,20,30q�67   $
��Exit��6M   5      �� 5    
   -   5        
 ��Name$

 text��5       $��15(�6   $��DemiBold"�6,   $��0��6B   $	��205��6O   $��0,0,0,90q�6\   $��2��6n   5!      5"      