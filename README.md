## ToDoList
# Тестовое задание

Необходимо реализовать возможность создавать, просматривать и удалять задачи. Требуется создать консольное приложение, которое обрабатывает команды:
- **/add** *task-info* - создает новую задачу
- **/all** - выводит все задачи
- **/delete id** - удаляет задачу по идентификатору (который должен отображаться в **/all**)
- **/save** *file-name.txt* - сохраняет все текущие задачи в указанный файл
- **/load** *file-name.txt* - загружает задачи с файла
- Указание, что задача выполнена
  - **/complete** *id* - выставляет, что задача выполнена
  - Выполненные задачи отображаются в конце списка и помечаются, что они выполнены
  - **/completed** - выводит все выполненные задачи
- Возможность указать дату выполнения (дедлайн)
  - Информация вывродиться в **/all**
  - Добавляется команда **/today** - выводит только те задачи, которые нужно сделать сегодня
- Подзадачи
  - Команда **/add-subtask** *id* *subtask-info* - добавляет к выбранной задаче подзадачу
  - Добавить поддержку выполнения подзадачи по команде **/complete** *id* 
  - Для задач с подзадачами выводится информация о том, сколько подзадач выполнено в формате "3/4"
- Обработка ошибок - отсутствие файлов, неправильный формат ввода
  - Учесть корнер кейсы. Например: задача не может быть добавлена дважды


## Пример input.txt
  + Wake Up 04.06.2021 
  + Breakfast  
	   * Cook 
	   * Understand
	   * Lol 
  + New Task 06.06.2021

## Пример output.txt 
  + Wake Up 4.6.2021 
  + Breakfast 0/3
	  * Cook 
	  * Understand 
	  * Lol 
 + New Task did 6.6.2021 

(То есть .(точка) перед задачей, *(звездочка) перед поздадачей)
