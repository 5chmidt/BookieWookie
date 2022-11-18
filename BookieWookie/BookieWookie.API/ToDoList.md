ToDoList
API EndPoints:
	/file/upload/  (upload cover images)

Add Authorization to Endpoints:
	/user/update/
	/user/delete/
	/user/getall/

Controller Unit Tests:
	/book/get
	/book/create
	/book/update
	/book/delete

	/user/get
	/user/authenticate/
	/user/create/
	/user/update/
	/user/delete/

	/file/upload/
	
XML Comment Documentation for Schemas
	AuthenticationRequest
	CreateBookRequest
	UserRequest


DevOps Wishlist
Create docker build
	- Image for Authentication only
		Slow and secure with dedicated resources
	- Image for API
		Fast async controller calls seperated from auth
	- SQL Express Image
		With EF migration setup in build
	- Load balancing for API