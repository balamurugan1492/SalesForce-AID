   M S B A M L   `   `   `  ����p  lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls  5   	    Pointel.Interactions.Chat3   -Pointel.Interactions.Chat.WinForms.MessageBox   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   cmyTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit     5   	   -   5        $��MsgWin��5   @     � AllowsTransparency  .  � WindowStyle$
 None=�5   "    � WindowStartupLocation$ CenterScreen=�65    � ShowInTaskbar .   � Topmost . 
������ $
��Auto��5       $	��370��6.    � SizeToContent$ Height=�6:    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5	      �� 5
      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      �� -   5        $��
MainBorder��5   B   $
��0.89q�6   $��8q�6$   
�������._�5   
   � 	 � RowDefinitions	 5      �� 5      $
��Auto �6    60   �� 5      $
��Auto �6    60   �� 5      $
��Auto �6    60   .+�5      �� -   5        $��lblTitle��5   !   $��0��6   $��14(�61   $��Stretch=�6?   $��Calibri)�6]   ���#007edf
�������6�   �� 5      $��1��6   $��2��6"   ���#656d90$	��0.5q�6I   $��5q�6_   � 5      $��1��6   $��50��6    $��10q�60   	 5      �� 5      $��35��6$   $��* �63   6@   �� 5      -   5        
 �Name$
 growFwd��5   $   $��0 �63   6@   .+�5      �� -   5        $��txtblockContent��5   )   $��0��6   $
��Wrap=�6@   $��Calibri)�6T   $��12(�6i   
���   �$��8q�6�   6�   �� 5       $��1��6    �� Orientation$ 
Horizontal=�6*   .+�5!      �� -   5        $��
lblForward��5!      $��50,0,0,0q�6.   $��Forward��6@   5$      5%      �� 5&      -   5        $��ForwardError��5&      $��2��6(   
���  ��$��Calibri)�6F   $	��Top=�6[   $��10,2,0,0q�6s   $
��Left=�6�   6�   �� 5'      -   5        $��	btn_right��5'      $��	0,0,10,10q�6&   $��2��69   $��Right=�6F   $��23��6b   $��80��6n      NormalButton#��[  6y   6�   �� 5(      -	   5        $��btn_left��5(      $��
0,0,100,10q�6%   $��2��69   $��Right=�6F   $��23��6b   $��80��6n   #��[  6y   6�   5)      5*      5,      