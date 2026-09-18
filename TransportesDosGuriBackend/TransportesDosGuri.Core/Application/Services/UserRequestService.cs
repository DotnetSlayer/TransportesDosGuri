using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;

namespace TransportesDosGuri.Core.Application.Services
{
    public class UserRequestService : IUserRequestService
    {
        private readonly IUserRequestRepository _userRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAsaasGateway _asaasGateway;

        public UserRequestService(
            IUserRequestRepository userRequestRepository,
            IUserRepository userRepository,
            IAsaasGateway asaasGateway)
        {
            _userRequestRepository = userRequestRepository;
            _userRepository = userRepository;
            _asaasGateway = asaasGateway;
        }

        public async Task<UserRequestDTO> CreateWithPaymentAsync(UserRequestDTO userRequestDto)
        {
            var user = await _userRepository.GetByIdAsync(userRequestDto.ApplicationUserId);
            if (user == null)
                throw new Exception("Requisição Inválida!");

            string customerId = user.CustomerAsaasId;

            if (string.IsNullOrEmpty(customerId))
            {
                customerId = await _asaasGateway.CreateCustomerAsync(
                    $"{user.Name} {user.LastName}",
                    user.Email,
                    user.PhoneNumber,
                    user.IdentityNumber
                );

                await _userRepository.UpdateAsaasCustomerIdAsync(user.Id, customerId);
            }

            var dueDate = userRequestDto.DueDate.ToDateTime(TimeOnly.MinValue);

            var (paymentId, _) = await _asaasGateway.CreatePaymentAsync(
                customerId,
                userRequestDto.Price,
                dueDate,
                $"Solicitação #{userRequestDto.Id}",
                $"userrequest:{userRequestDto.Id}"
            );

            var userRequestEntity = new UserRequest
            {
                Price = userRequestDto.Price,
                DueDate = userRequestDto.DueDate,
                ApplicationUserId = userRequestDto.ApplicationUserId,
                AsaasSubscriptionId = paymentId
            };

            await _userRequestRepository.AddAsync(userRequestEntity);

            userRequestDto.Id = userRequestEntity.Id;
            userRequestDto.AsaasSubscriptionId = paymentId;

            return userRequestDto;
        }

        public async Task<UserRequestDTO> CreateAsync(UserRequestDTO userRequest)
        {
            var userRequestEntity = new UserRequest
            {
                Price = userRequest.Price,
                DueDate = userRequest.DueDate,
                ApplicationUserId = userRequest.ApplicationUserId,
                AsaasSubscriptionId = userRequest.AsaasSubscriptionId
            };

            await _userRequestRepository.AddAsync(userRequestEntity);

            userRequest.Id = userRequestEntity.Id;

            return userRequest;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var userRequestEntity = await _userRequestRepository.GetByIdAsync(id);

            if (userRequestEntity == null)
            {
                return false;
            }

            await _userRequestRepository.DeleteAsync(id);

            return true;
        }

        public async Task<IEnumerable<UserRequestDTO>> GetAllAsync()
        {
            var userRequestEntities = await _userRequestRepository.GetAllAsync();

            return userRequestEntities.Select(entity => new UserRequestDTO
            {
                Id = entity.Id,
                Price = entity.Price,
                DueDate = entity.DueDate,
                ApplicationUserId = entity.ApplicationUserId,
                AsaasSubscriptionId = entity.AsaasSubscriptionId
            });
        }

        public async Task<UserRequestDTO?> GetByIdAsync(long id)
        {
            var userRequestEntity = await _userRequestRepository.GetByIdAsync(id);

            if (userRequestEntity == null)
            {
                return null;
            }

            return new UserRequestDTO
            {
                Id = userRequestEntity.Id,
                Price = userRequestEntity.Price,
                DueDate = userRequestEntity.DueDate,
                ApplicationUserId = userRequestEntity.ApplicationUserId,
                AsaasSubscriptionId = userRequestEntity.AsaasSubscriptionId
            };
        }

        public async Task<bool> UpdateAsync(long id, UserRequestDTO userRequest)
        {
            var existingUserRequest = await _userRequestRepository.GetByIdAsync(id);

            if (existingUserRequest == null)
            {
                return false;
            }

            existingUserRequest.Price = userRequest.Price;
            existingUserRequest.DueDate = userRequest.DueDate;
            existingUserRequest.ApplicationUserId = userRequest.ApplicationUserId;
            existingUserRequest.AsaasSubscriptionId = userRequest.AsaasSubscriptionId;

            await _userRequestRepository.UpdateAsync(existingUserRequest);

            return true;
        }
    }
}