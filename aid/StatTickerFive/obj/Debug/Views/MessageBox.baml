   M S B A M L   `   `   `  ����c  _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity  5   	    StatTickerFive%   StatTickerFive.Views.MessageBox   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Ge9http://schemas.microsoft.com/expression/2010/interactions     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   -   5        $��	MsgWindow��5	   	     � AllowsTransparency  .  � WindowStyle$
 None=�5
   "    � WindowStartupLocation$ CenterScreen=�65    � ShowInTaskbar .   � Topmost . $	��150��5   	   $	��380��6    � SizeToContent$ WidthAndHeight=�6"    � Icon$B </StatTickerFive;component/Images/StatTickerFive-32x32-01.png��6A   ��5
      �� *BackgroundColor+ J� BitmapEffect 5   �   �� *ShadowEffect+.   (System.Windows.Interactivity.Interaction   Triggers 5      /   )System.Windows.Interactivity.EventTrigger  5   
   	   	EventName$	 	Activated��6   .   (System.Windows.Interactivity.TriggerBase
   Actions.
 5      6   0System.Windows.Interactivity.InvokeCommandAction     Command 6$   �� *ActivatedCommand+6K   5        5   
   $	 Deactivated��6   .
 5         6$   �� *DeactivateCommand+6L   5        5   
   $	 MouseLeftButtonDown��6   .
 5         6$   �� *	DragCmd+6B   5       � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $< 6/StatTickerFive;component/Resources/StyleResource.xaml?�5      %    5      .��5!      �� $
��0.89q�6   
�������$
��Auto��68   $	��380��6F   ��6R   �� *BorderBrush+._�5"   
   �  � RowDefinitions 5#      �� 5$      $��25 �6    6.   �� 5%      $��* �6    6-   �� 5&      $��30 �6    6.   .+�5(      �� $��0��6   $��14(�6!   $��Stretch=�6/   $��Calibri)�6M   $��0,-3,0,0q�6�   ��6b   �� *TitleBackground+��6�   �� *TitleForeground+��6�   �� *MessageboxTitle+6�   �� 5)      -   5        $��btnExit��5)   >   $��0��6   $��Right=�6"   $��20��6t   $��20��6�      AdminCloseButton#��[  6�   $��2q�6�   y�6M   �� *CloseVisibility+��6�   �� *MsgCancelCmd+�� 5*      $��1��6   $
��Auto��6"   $
��Auto��60   $��2��6=   ���#656d90$	��0.5q�6d   $��5q�6z   � 5+      $��1��6   $��10q�6!   .+�5,      �� $
��Wrap=�6   $��Calibri)�61   $��15(�6F   
���   �$��8q�6g   ��6r   �� *MessageboxContent+6�   5-      �� 5.      -   5        $��
leftbutton��5.      $��2��6'   $��23��6�   $
��Left=�6�   $	��Top=�6�   $��75��6�   $��
200,-2,8,0q�6�     NormalButton#��[ 6�   y�64   �� *leftbuttonVisible+��6]   �� *leftbuttonContent+��6	  �� *MsgBoxClose+��6)  ��  �� ElementName$ 
leftbutton�� �� Path$ Content��� 5/      -   5        $��rightbutton��5/      $��2��6(   $��23��6�   $��Right=�6�   $	��Top=�6�   $��75��6�   $��0,-2,8,0q�6�   #��[ 6�   y�65   �� *rightbuttonVisible+��6_   �� *rightbuttonContent+��6  �� *MsgBoxClose+��6+  �� $ rightbutton��$ Content�50      51      53      