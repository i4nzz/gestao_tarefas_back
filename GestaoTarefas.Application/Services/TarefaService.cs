using System.Net;
using GestaoTarefas.Application.Common.Responses;
using GestaoTarefas.Application.DTOs.Tarefa;
using GestaoTarefas.Application.Interfaces;
using GestaoTarefas.Application.Mapping;
using GestaoTarefas.Domain.Entities;
using GestaoTarefas.Domain.Interfaces;

namespace GestaoTarefas.Application.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAutorizacaoFamiliarService _autorizacao;


    public TarefaService(
        ITarefaRepository tarefaRepository
        , IUsuarioRepository usuarioRepository
        , IAutorizacaoFamiliarService autorizacao
        )
    {
        _tarefaRepository = tarefaRepository;
        _usuarioRepository = usuarioRepository;
        _autorizacao = autorizacao;
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoTarefaDto>>> ObterTodasAsync()
    {
        var tarefas = await _tarefaRepository.ObterTodasAsync();

        if (tarefas == null || !tarefas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma tarefa encontrada"
            };
        }

        // "ObterTodas" retorna apenas as tarefas da família do usuário autenticado,
        // não existe perfil de administrador com visão de todas as famílias.
        var autorizadas = new List<Tarefa>();
        var acessoPorFilho = new Dictionary<int, bool>();

        foreach (var tarefa in tarefas)
        {
            if (!acessoPorFilho.TryGetValue(tarefa.FilhoId, out var podeAcessar))
            {
                podeAcessar = await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId);
                acessoPorFilho[tarefa.FilhoId] = podeAcessar;
            }

            if (podeAcessar)
            {
                autorizadas.Add(tarefa);
            }
        }

        if (!autorizadas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma tarefa encontrada"
            };
        }

        var retornoTarefas = autorizadas.ToDtoList();

        return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoTarefas,
            Mensagem = "Tarefas obtidas com sucesso"
        };
    }

    public async Task<RespostaMetodos<IEnumerable<RetornoTarefaDto>>> ObterPorFilhoAsync(int filhoId)
    {
        if (!await _autorizacao.PodeAcessarFilhoAsync(filhoId))
        {
            return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar as tarefas deste filho"
            };
        }

        var tarefas = await _tarefaRepository.ObterPorFilhoAsync(filhoId);

        if (tarefas == null || !tarefas.Any())
        {
            return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Nenhuma tarefa encontrada para o filho especificado"
            };
        }

        var retornoTarefas = tarefas.ToDtoList();

        return new RespostaMetodos<IEnumerable<RetornoTarefaDto>>
        {
            Sucesso = true,
            ObjetoRetorno = retornoTarefas,
            Mensagem = "Tarefas obtidas com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoTarefaDto?>> ObterPorIdAsync(int tarefaId)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<RetornoTarefaDto?>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoTarefaDto?>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para acessar esta tarefa"
            };
        }

        var retornoTarefa = tarefa.ToDto();

        return new RespostaMetodos<RetornoTarefaDto?>
        {
            Sucesso = true,
            ObjetoRetorno = retornoTarefa,
            Mensagem = "Tarefa obtida com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoTarefaDto>> CriarAsync(CriarTarefaDto dto)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(dto.FilhoId);

        if (usuario == null)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Filho não encontrado"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode criar tarefas para um filho que não é vinculado a você"
            };
        }

        if (dto.Prazo <= DateTime.UtcNow)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "O prazo da tarefa deve ser uma data futura"
            };
        }

        if (dto.Pontos <= 0)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Os pontos da tarefa devem ser maiores que zero"
            };
        }

        var tarefa = new Tarefa
        {
            FilhoId = dto.FilhoId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            Pontos = dto.Pontos,
            Prazo = dto.Prazo
        };

        var retornoTarefa = tarefa.ToDto();

        await _tarefaRepository.AdicionarAsync(tarefa);

        return new RespostaMetodos<RetornoTarefaDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoTarefa,
            Mensagem = "Tarefa criada com sucesso"
        };
    }

    public async Task<RespostaMetodos<RetornoTarefaDto>> AtualizarAsync(int tarefaId, CriarTarefaDto dto)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);

        if (tarefa == null)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para editar esta tarefa"
            };
        }

        // se o dto estiver tentando mover a tarefa pra outro filho, valida o novo dono também
        if (dto.FilhoId != tarefa.FilhoId && !await _autorizacao.PodeAcessarFilhoAsync(dto.FilhoId))
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não pode mover esta tarefa para um filho que não é vinculado a você"
            };
        }

        if (dto.Prazo <= DateTime.UtcNow)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "O prazo da tarefa deve ser uma data futura"
            };
        }

        if (dto.Pontos <= 0)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Os pontos da tarefa devem ser maiores que zero"
            };
        }

        tarefa.Titulo = dto.Titulo;
        tarefa.FilhoId = dto.FilhoId;
        tarefa.Descricao = dto.Descricao;
        tarefa.Pontos = dto.Pontos;
        tarefa.Prazo = dto.Prazo;

        await _tarefaRepository.AtualizarAsync(tarefa);

        var retornoTarefa = tarefa.ToDto();

        return new RespostaMetodos<RetornoTarefaDto>
        {
            Sucesso = true,
            ObjetoRetorno = retornoTarefa,
            Mensagem = "Tarefa atualizada com sucesso"
        };
    }
    public async Task<RespostaMetodos<RetornoTarefaDto>> RemoverAsync(int tarefaId)
    {
        var tarefa = await _tarefaRepository.ObterPorIdAsync(tarefaId);
        if (tarefa == null)
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                ObjetoRetorno = null,
                Mensagem = "Tarefa não encontrada"
            };
        }

        if (!await _autorizacao.PodeAcessarFilhoAsync(tarefa.FilhoId))
        {
            return new RespostaMetodos<RetornoTarefaDto>
            {
                Sucesso = false,
                StatusCode = HttpStatusCode.Forbidden,
                Mensagem = "Você não tem permissão para remover esta tarefa"
            };
        }

        await _tarefaRepository.RemoverAsync(tarefaId);

        return new RespostaMetodos<RetornoTarefaDto>
        {
            Sucesso = true,
            ObjetoRetorno = null,
            Mensagem = "Tarefa removida com sucesso"
        };
    }
}