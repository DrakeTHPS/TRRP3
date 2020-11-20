from concurrent import futures
import grpc
import quadratic_pb2_grpc
import quadratic_pb2
import configparser


class QuadraticServiceImpl(quadratic_pb2_grpc.QuadraticServicer):
    def Calculate(self, request, context):
        result = quadratic_pb2.QuadraticResponse()
        operandA = request.operandA
        operandB = request.operandB
        operandC = request.operandC
        desc = operandB ** 2 - 4*operandA*operandC
        if desc < 0:
            result.quadraticStatus = quadratic_pb2.FAILED
        else:
            result.root1 = (-operandB + desc ** 0.5) / (2*operandA)
            result.root2 = (-operandB - desc ** 0.5) / (2*operandA)
            result.quadraticStatus = quadratic_pb2.OK
        return result


def serve():
    config = configparser.ConfigParser()
    config.read('connection.ini')
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    quadratic_pb2_grpc.add_QuadraticServicer_to_server(
        QuadraticServiceImpl(), server)
    server.add_insecure_port(f'[::]:{config["server"]["port"]}')
    server.start()
    print('Погнали')
    server.wait_for_termination()


if __name__ == '__main__':
    serve()
