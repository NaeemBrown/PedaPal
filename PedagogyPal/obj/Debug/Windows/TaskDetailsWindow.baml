   M S B A M L   `   `   `  ����  PedagogyPal+    %PedagogyPal.Windows.TaskDetailsWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Task Details��5   	   $	��300��6   $	��400��6+   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5	      $
��Auto �6   �� 5
      $��* �6   .+�5   
   �� $��Title:��6   $��0��6"   $��Center=�6/   �� 5   
   $��0��60   $��	100,0,0,0q�6=   $��Center=�6P   ��6   �� *
Task.Title+6P   �� 5   
   $��	Due Date:��6   $��1��6%   $��Center=�62   $��0,10,0,0q�6M   �� 5   
   $��1��62   $��
100,10,0,0q�6?   $��Center=�6S   ��6   �� *Task.DueDate+6S   �� 5   
   $��Status:��6   $��2��6#   $��Center=�60   $��0,10,0,0q�6K   �� 5   
   $��2��61   $��
100,10,0,0q�6>   $��Center=�6R   ��6   �� *Task.Status+6R   �� 5   
   $��Attachment:��6   $��3��6'   $	��Top=�64   $��0,10,0,0q�6L   �� 5   
   $
��Open��6   $��3��6    $��
100,10,0,0q�6-   $	��100��6A   ��6M   �� *DownloadDocumentCommand+��6y   �� *Task.DocumentLink+5      5      