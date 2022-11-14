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
	
Swagger Configurations:
	Add bearer token options where required
	For non-dev builds add login page to access swagger documentation.

XML Comment Documentation for Schemas
	AuthenticationRequest
	CreateBookRequest
	UserRequest


DevOps Wishlist
Create docker build
	- Image for Authentication only
		Slow and secure with dedicated resources
	- Image for API
		Fast async controller calls
	- SQL Express Image
		With EF migration setup in build
	- Load balancing for API