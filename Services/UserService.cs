using HashPasswordApi.Data;
using HashPasswordApi.Model;
using HashPasswordApi.Resources;
using System.Data.Entity;

namespace HashPasswordApi.Services
{
    public class UserService
    {
        private readonly DataContext _context;
        private readonly string _pepper;
        private readonly int _iteration = 3;
        public UserService(DataContext context)
        {
            _context = context;
            _pepper = Environment.GetEnvironmentVariable("PasswordHashExamplePepper");
        }

        public async Task<UserResource> Register(RegisterResource resource, CancellationToken cancellationToken)
        {
            var user = new Users
            {
                Username = resource.Username,
                Email = resource.Email,
                PasswordSalt = PasswordHasher.GenerateSalt()
            };
            user.PasswordHash = PasswordHasher.ComputeHash(resource.Password, user.PasswordSalt, _pepper, _iteration);
            await _context.users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new UserResource(user.Id, user.Username, user.Email);
        }

        public async Task<UserResource> Login(LoginResource resource, CancellationToken cancellationToken)
        {
            var user = await _context.users
                .FirstOrDefaultAsync(x => x.Username == resource.Username, cancellationToken);

            if (user == null)
                throw new Exception("Username or password did not match.");

            var passwordHash = PasswordHasher.ComputeHash(resource.Password, user.PasswordSalt, _pepper, _iteration);
            if (user.PasswordHash != passwordHash)
                throw new Exception("Username or password did not match.");

            return new UserResource(user.Id, user.Username, user.Email);
        }
    }
}
