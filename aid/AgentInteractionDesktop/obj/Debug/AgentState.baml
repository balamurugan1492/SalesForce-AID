   M S B A M L   `   `   `  ����U  QAgent.Interaction.Desktop, Version=5.0.3.27, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  5      *    $Agent.Interaction.Desktop.AgentState   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      NCustomTimer6clr-namespace:Agent.Interaction.Desktop.CustomControls     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      6my-http://schemas.microsoft.com/wpf/2008/toolkit  5      -   5          � 	Resources  5	       �� MergedDictionaries �� Source�� 5
   
    5      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      � 
������ .+�5   
   �� ���#ADAAAD$��1q�6'   $��10q�6;   $
��Auto��6G   $
��Auto��6T   
�������._�5      �  � ColumnDefinitions 5      �� 5      $
��Auto �6'   66   �� 5      $
��Auto �6'   66    � RowDefinitions 5      �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   �� 5      $
��Auto �6$   64   .+�5       ��  �� Orientation$ 
Horizontal=�6   $��2��66   $��0��6J   $
��Left=�6W   $��10,5,0,5q�6r   .+�5!      �� -   5        $��img_agentStatus��5!      $��10��63   $��10��6?   ��6J   �� *VoiceStateImageSource+6u   �� 5"      -   5        $��lblagentName��5"       $
��Bold"�64   $��12(�6F   $��5,0,0,0q�6s   ��6T   �� *AgentFullName+6�   5#      �� 5(      -   5        $��lblLoggedIN_time��5(      $��2��64   $��1��6H   $
��Left=�6U   $��40,3,0,0q�6p   6�   �� 5)      -   5        $��
lblLoginID��5)      $��2��6.   $��2��6B   $
��Left=�6O   $��40,3,0,0q�6j   6~   �� 5*      -   5        $��lblagentPlace��5*      $��2��61   $��3��6E   $
��Left=�6R   $��40,3,0,0q�6m   ��6   �� *	UserPlace+6�   )  #Microsoft.Windows.Controls.DataGrid  5+      -   5        $��DGMyChannels��5+   �      RowHeaderWidth$ 0Q�6   
������ $��	10,10,5,5q�6K   
�������$
��Left=�6q   $��Center=�5,      $
��Left=�60      HorizontalGridLinesBrush
 ����    VerticalGridLinesBrush
 ���� 	   AutoGenerateColumns	 .  
   CanUserAddRows
 .     CanUserDeleteRows .  $��2��5.   W   $��4��6k      HeadersVisibility$
 None=�6x   ��63   �� *MediaStatus+   Columns 5/      7  1Microsoft.Windows.Controls.DataGridTemplateColumn  50      /  )Microsoft.Windows.Controls.DataGridColumn   Header$ Media��64      Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto 6C      MinWidth$ 70Q�6P      CellTemplate 51      �� 52   "   .R�53   &   �� $ 
Horizontal=�61   .+�54   *   �� $��imgChannelIcon��60   $��15��6F   $��15��6R   ��6]   �� *ChannelIconImageSource+�� 55   *   $��txtChannelName��6O   $
��Left=�64   $��2,0,0,0q�6�   ��6e   �� *ChannelName+6�   56   '   57   #   59        5:      $ Status��64   $
 Auto 6D   $	 130Q�6Q    5;      �� 5<   "   .R�5=   &   �  5>   *   �� 5?   .   $
��Auto �6?   6N   �� 5@   .   $
��Auto �6?   6N   �� 5A   .   $
��Auto �6?   6N   .+�5C   *   �� $��imgChannelStateIcon��6[   $��0��60   $
��Left=�6@   $��5,0,5,0q�6v   $��10��6�   $��10��6�   ��6�   �� *ChannelStateImageSource+�� 5D   *   $��txtChannelState��6_   $��1��64   $
��Left=�6D   $��2,0,0,0q�6�   ��6v   �� *ChannelState+6�   9   3Agent.Interaction.Desktop.CustomControls.TimerLabel  5E   *   $��5,0,5,0q�6A   $��2��6R   $��Right=�6b      Text 6   �� *ChannelInitialTime+y�6�   �� *
StateTimer+5F   '   5G   #   5I         RowStyle 5K      �� 5L       �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"  6     �� Triggers 5M      P� 5N   "    P� Property �@ 
IsSelected P� Value . . �5O   &   ��  �� Property � ��   HighlightedBrush �� Value# [  6C   5P   #   5R      5T      5U      5V      5W      5X      