   M S B A M L   `   `   `  ���� '  #Pointel.Windows.Views.Common.EditorE    ?Pointel.Windows.Views.Common.Editor.Controls.PrintPreviewWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     -     � Title$  Print Preview�� � AllowsTransparency .  � WindowStyle$
 None=� � Topmost .   � 
ResizeMode$ CanMinimize=�
������  � Icon$G A/Pointel.Windows.Views.Common.Editor;component/Images/printer.ico�� � 	Resources    NormalBrush �� 
StartPoint �� EndPoint	 � Color
 � Offset  NormalBorderBrush  HorizontalNormalBorderBrush  HorizontalNormalBrush  ScrollBarLineButton �� 
TargetType �� Property �� Value �� 
TargetType �� Path �� RelativeSource �� Triggers P� Property P� Value �� 
TargetName  ScrollBarPageButton  ScrollBarThumb  VerticalScrollBar � RowDefinitions ]� DecreaseRepeatButton ]� Thumb ]� IncreaseRepeatButton  HorizontalScrollBar � ColumnDefinitions 	 CloseButton ��Name �� Triggers �� Value �� Binding �� Mode �� AncestorType  � 	Resources 
 Key! g� 	BeginTime" �� 
TargetName# �� TargetProperty$ O� KeyTime% O� Value& J� SnapsToDevicePixels' �� ScaleX( �� ScaleY) �� AngleX* �� AngleY+ �� Angle, T� X- T� Y. :� RoutedEvent  OnMouseEnter1  RestoreButton  MinimizeButton  maximizeButton��%�  &	        &	 V     &	 �     &	     &	 X    &	 O    &	     &	 �    7  7  7  7  7  7  7  &	 g    7  7  7  7   7  7  7  &		 	    7  &	 G    7  &	 d    7  &	 �    7  �� $	 0,0&�$	 0,1&���� � $
	 #FFF��$	
 0.0Q�� $
	 #CCC��$	
 1.0Q��� $	 0,0&�$	 0,1&���� � $
	 #CCC��$	
 0.0Q�� $
	 #444��$	
 1.0Q��� $	 0,0&�$	 1,0&���� � $
	 #CCC��$	
 0.0Q�� $
	 #444��$	
 1.0Q��� $	 0,0&�$	 1,0&���� � $
	 #FFF��$	
 0.0Q�� $
	 #CCC��$	
 1.0Q��� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � |� .  ��  � �� �� " ��.R��� $��Border��$��1q�
����������#ADAAAD$��1q�._�F� $��Center=�$��Center=����#737573���� $ Content� �� *TemplatedParent+ P�  �@J�IsMouseOver . . ��� $ Border�� � ��
 �   �P�  � �� . . ��� $ Border�� � ��
 �   ��� $ Border�� � �� �#F7F3F7P�  � {� .  �� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " ��.R��� ���#F7F3F7�� " n�.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . ��  � �� .  ��  � |� .  ��  � �� �� " n�.R��� $��Border��
����������#ADAAAD$��1q� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#F7F3F7P�  �@n�
IsDragging . . ��� $ Border�� � �� �#F7F3F7�� $ Border�� � ��
 �   �P�  � {� .  �� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineUpCommand��$��M 0 4 L 8 4 L 4 0 Z��]� $��
PART_Track��$��1����.  �� 8�� $��ScrollBar.PageUpCommand�� n� 8�� $��1,0,1,0q�8�� 8��  �� 8�� $��ScrollBar.PageDownCommand���� $��3��8�� $��18��$��ScrollBar.LineDownCommand��$��M 0 0 L 4 4 L 8 0 Z���� " ��.R��  �� $��18���� $��0.00001* ��� $��18��.+��� $��3�����#F0F0F0�� $��0��8��  $��18��$��ScrollBar.LineLeftCommand��$��M 4 0 L 4 8 L 0 4 Z��]� $��
PART_Track��$��1����.   �� 8�� $��ScrollBar.PageLeftCommand�� n� 8�� $��0,1,0,1q�8�� 8��  �� 8�� $ ��ScrollBar.PageRightCommand���� $��3��8�� $��18��$ ��ScrollBar.LineRightCommand��$��M 0 0 L 4 4 L 0 8 Z���� " ��.���  � �� �� .R��� $ Close��$L��F/Pointel.Windows.Views.Common.Editor;component/Images/Custom Close.png��$��10��$��10�� �� $
 True�� ��  �� $ FindAncestor=�" ��$ IsMouseOver�.a��� $ Close�� � ��$U O/Pointel.Windows.Views.Common.Editor;component/Images/Custom Close selected.png���� $
 True�� ��  �� $ FindAncestor=�" ��$ 	IsPressed�.a��� $ Close�� � ��$U O/Pointel.Windows.Views.Common.Editor;component/Images/Custom Close selected.png����  � �� �� " ��  ���� OnMouseEnter1
 .��U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $' 1Q�$( 1Q��� $) 0Q�$* 0Q��� $+ 0Q�T� $, 0Q�$- 0Q� :� $. Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� .R��� $ Minimize��$N��H/Pointel.Windows.Views.Common.Editor;component/Images/Normal.Disable.png��$��15��$��11�� �� $
 True�� ��  �� $ FindAncestor=�" ��$ IsMouseOver�.a��� $ Minimize�� � ��$F @/Pointel.Windows.Views.Common.Editor;component/Images/Normal.png���� $
 True�� ��  �� $ FindAncestor=�" ��$ 	IsPressed�.a��� $ Minimize�� � ��$N H/Pointel.Windows.Views.Common.Editor;component/Images/Normal.Disable.png����  � �� �� " ��  ���� OnMouseEnter1
 .��U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $' 1Q�$( 1Q��� $) 0Q�$* 0Q��� $+ 0Q�T� $, 0Q�$- 0Q� :� $. Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� .R��� $ Minimize��$O��I/Pointel.Windows.Views.Common.Editor;component/Images/Custom Minimize.png��$��0,-1,0,0q�$��14��$��11�� �� $
 True�� ��  �� $ FindAncestor=�" ��$ IsMouseOver�.a��� $ Minimize�� � ��$X R/Pointel.Windows.Views.Common.Editor;component/Images/Custom Minimize selected.png���� $
 True�� ��  �� $ FindAncestor=�" ��$ 	IsPressed�.a��� $ Minimize�� � ��$X R/Pointel.Windows.Views.Common.Editor;component/Images/Custom Minimize selected.png����  � �� �� " ��  ���� OnMouseEnter1
 .��U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $' 1Q�$( 1Q��� $) 0Q�$* 0Q��� $+ 0Q�T� $, 0Q�$- 0Q� :� $. Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " ��.���  � �� �� .R��� $ Minimize��$Q��K/Pointel.Windows.Views.Common.Editor;component/Images/Maximize1.Disable.png��$��15��$��11�� �� $
 True�� ��  �� $ FindAncestor=�" ��$ IsMouseOver�.a��� $ Minimize�� � ��$I C/Pointel.Windows.Views.Common.Editor;component/Images/Maximize1.png���� $
 True�� ��  �� $ FindAncestor=�" ��$ 	IsPressed�.a��� $ Minimize�� � ��$I C/Pointel.Windows.Views.Common.Editor;component/Images/Maximize1.png����  � �� �� " ��  ���� OnMouseEnter1
 .��U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�U� $! 00:00:00X�$" content��$V# P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�.\��� $$ 00:00:00��$	% 1.4Q��� $$ 00:00:01��$% 1Q�.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z��z�V� .~��� $' 1Q�$( 1Q��� $) 0Q�$* 0Q��� $+ 0Q�T� $, 0Q�$- 0Q� :� $. Mouse.MouseEnter��.[��� 8��  P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7.��� .+��� -   $��
MainBorder��$��1q�$��2q�
�������._��  �� $
��Auto ��� $��* �.+�� -   $��0�� �� $��* ��� $
��Auto ��� $
��Auto �.+��� $��0��$��5,0,0,0q�
�������$
��Auto��._��� / �� Orientation$/ 
Horizontal=�.+��� -   $��imgChatAgentInfo��$��16��$��Center=�$E��?/Pointel.Windows.Views.Common.Editor;component/Images/Print.png��$��1q�0 � ToolTip$0 Print Preview���� -   $��txtChatStatus��$��2,2,5,2q�$��Calibri)�$��14(����#007EDF$
��Bold"�$��Stretch=�$��Print Preview��$��Center=��� $��1���� $��2��$��60��$
��Auto��$��Right=�$	��Top=�$/ 
Horizontal=�.+��� -   $��btnMinimize��$��16��$��20��#��[ �� -   $��btnMaximize��$��16��$��20��#��[ �� -   $��btnExit��$��16��$��20��#��[	 ]� -	   $��DocumentPanel��$��1��.+�Z� -
   $��	docViewer��$��2q����#D6D7D6$��1q�
�������   NormalImageButton1 �� Triggers��%�  '��       &	 *     'c� ;    '�� x    7  7  �� " ��.���  � y�$ 	Collapsed=��� " ��.���  � �� �� " ��.R��� $��Border��
������ ._��� $��content��#��z��#��z����. #& z& #��z�� P�  �@J�IsMouseOver . . ��� $ Border�� � �� �#d6ebffP�  � �� . . ��� $ Border�� � �� �#94c3e7�� " c�.���  � ��
 �   ���  � ��
 ������� " ��.���  �@J�SnapsToDevicePixels . ��  �@�OverridesDefaultStyle . 1 P�  �@��Orientation$ Vertical=�. ���  � ��$ 18����  � ��$
 Auto����  � ��8   P�  �@��Orientation$ 
Horizontal=�. ���  � ��$
 Auto����  � ��$ 18����  � ��8  