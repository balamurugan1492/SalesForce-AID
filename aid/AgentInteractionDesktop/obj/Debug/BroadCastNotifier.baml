   M S B A M L   `   `   `  ����S  OPointel.TaskbarNotifier, Version=5.0.1.50, Culture=neutral, PublicKeyToken=nullbFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifierPointel.TaskbarNotifier  5   	    Agent.Interaction.Desktop1   +Agent.Interaction.Desktop.BroadCastNotifier   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   [NotifierFclr-namespace:Pointel.TaskbarNotifier;assembly=Pointel.TaskbarNotifier     5   	   6my-http://schemas.microsoft.com/wpf/2008/toolkit  5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � 
ResizeMode$  NoResize=�5   @    � AllowsTransparency .  � WindowStyle$
 None=�6p   
������  � Title$ BroadCastNotifier��5   	   $
��Auto��6#   $	��400��61    � SizeToContent$ Height=�6=    � Topmost .  � 	Resources 5       �� MergedDictionaries �� Source�� 5	   
    5
      �� 5      $R L/Agent.Interaction.Desktop;component/Resources/StylesResourceDictionary.xaml?�5      %    5      .��5      �� -   5        $��
MainBorder��5   X   $
��0.89q�6   $��8q�6$   
����������#0070C5._�5   
   � 	 � RowDefinitions	 5      �� 5      $
��Auto �6    60   �� 5      6"   �� 5      $
��Auto �6    60   .+�5      �� $��0��6   $��14(�6!   $��Calibri)�6/   ���#007edf
���������6l   �� *MessageType+6�   �� 5      $��1��6   $��2q�6"   ��6-   �� *BroadCastBackgroundBrush+._�5      � -   5        $��grd_Details��5      
�������$��1,10,1,1q�6=   
 J� Opacity$	
 0.9Q�6O    � ColumnDefinitions 5      �� 5      $��70 �6+   68   �� 5      6-   .+�5      �� $��Center=�6   $��Center=�67   $��50��6T   $��50��6_   ��6k   �� *MessageIconImageSource+6�   �� 5      -   5        $��Details��5   ]   $��1��6!   $��5,0,0,0q�61   $��Center=�6B    �� Orientation$ Vertical=�6l   .+�5       �� $��12(�6?   $��Calibri)�6M   $
��Bold"�6b   ��6%   �� *
Message1+6b   �� 5!      $��12(�6?   $��Calibri)�6M   ��6%   �� *
Message2+6M   �� 5"      $��12(�6?   $��Calibri)�6M   ��6%   �� *
Message3+6M   �� 5#      $��12(�6?   $��Calibri)�6M   ��6%   �� *
Message4+6M   5$      5%      5&      �� 5'      -   5        $��btn_Show��5'   4   $��2��6   $��5,5,90,5q�6"   $y�Visible=�6D      NormalButton#��[  6Y   $��Right=�6   $��23��6�   $��80��6�   $��_Show��6�   6�   �� 5(      -   5        $��btn_Dismiss��5(   -   $��2��6   $��5q�6"   #��[  6@   $��Right=�6f   $��23��6�   $��80��6�   $��_Dismiss��6�   6�   5)      5*      5+      