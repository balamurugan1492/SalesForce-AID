   M S B A M L   `   `   `  ���� U  QAgent.Interaction.Desktop, Version=5.0.3.30, Culture=neutral, PublicKeyToken=nullc6clr-namespace:Agent.Interaction.Desktop.CustomControls(Agent.Interaction.Desktop.CustomControls  *    $Agent.Interaction.Desktop.AgentState   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     NCustomTimer6clr-namespace:Agent.Interaction.Desktop.CustomControls     @d2http://schemas.microsoft.com/expression/blend/2008     6my-http://schemas.microsoft.com/wpf/2008/toolkit  -     � 	Resources   �� MergedDictionaries �� Source��  �� $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�%    .��� 
������ .+��� ���#ADAAAD$��1q�$��10q�$
��Auto��$
��Auto��
�������._��  � ColumnDefinitions �� $
��Auto ��� $
��Auto � � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto �.+���  �� Orientation$ 
Horizontal=�$��2��$��0��$
��Left=�$��10,5,0,5q�.+��� -   $��img_agentStatus��$��10��$��10������ *VoiceStateImageSource+�� -   $��lblagentName��$
��Bold"�$��12(�$��5,0,0,0q����� *AgentFullName+�� -   $��lblLoggedIN_time��$��2��$��1��$
��Left=�$��40,3,0,0q��� -   $��
lblLoginID��$��2��$��2��$
��Left=�$��40,3,0,0q��� -   $��lblagentPlace��$��2��$��3��$
��Left=�$��40,3,0,0q����� *	UserPlace+)  #Microsoft.Windows.Controls.DataGrid  -   $��DGMyChannels��   RowHeaderWidth$ 0Q�
������ $��	10,10,5,5q�
�������$
��Left=�$��Center=�$
��Left=�   HorizontalGridLinesBrush
 ����    VerticalGridLinesBrush
 ���� 	   AutoGenerateColumns	 .  
   CanUserAddRows
 .     CanUserDeleteRows .  $��2��$��4��   HeadersVisibility$
 None=����� *MediaStatus+   Columns 7  1Microsoft.Windows.Controls.DataGridTemplateColumn  /  )Microsoft.Windows.Controls.DataGridColumn   Header$ Media��   Width8  2Microsoft.Windows.Controls.DataGridLengthConverter$
 Auto    MinWidth$ 70Q�   CellTemplate �� .R��� $ 
Horizontal=�.+��� $��imgChannelIcon��$��15��$��15������ *ChannelIconImageSource+�� $��txtChannelName��$
��Left=�$��2,0,0,0q����� *ChannelName+  $ Status��$
 Auto $	 130Q� �� .R��  �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��imgChannelStateIcon��$��0��$
��Left=�$��5,0,5,0q�$��10��$��10������ *ChannelStateImageSource+�� $��txtChannelState��$��1��$
��Left=�$��2,0,0,0q����� *ChannelState+9   3Agent.Interaction.Desktop.CustomControls.TimerLabel  $��5,0,5,0q�$��2��$��Right=�   Text �� *ChannelInitialTime+y��� *
StateTimer+   RowStyle ��  �� 
TargetType,  &Microsoft.Windows.Controls.DataGridRow"   �� Triggers P�  P� Property �@ 
IsSelected P� Value . . ���  �� Property � ��   HighlightedBrush �� Value# [  