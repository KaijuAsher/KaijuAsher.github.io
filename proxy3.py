import socket

def proxy_server(host, port):
    # create a socket object
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    # bind the socket to a public host and port
    server_socket.bind((host, port))
    
    # become a server socket
    server_socket.listen(1)
    
    print(f"Proxy server running on {host}:{port}")
    
    while True:
        # accept connections from outside
        client_socket, client_address = server_socket.accept()
        
        # receive data from the client
        request = client_socket.recv(1024)
        
        # forward the request to the destination server
        destination_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        destination_socket.connect(('www.example.com', 80))
        destination_socket.sendall(request)
        
        # receive data from the destination server
        response = destination_socket.recv(1024)
        
        # send the response back to the client
        client_socket.sendall(response)
        
        # close the sockets
        destination_socket.close()
        client_socket.close()

# start the proxy server on localhost:8888
proxy_server('localhost', 8888)
