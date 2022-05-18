/*
*	<description>ObjDBInfo, atributos comuns para todos os objetos da camada "BO"</description>
* 	<author>João Carlos Pinto</author>
*   <date>23-03-2022</date>
*	<copyright>Copyright (c) 2022 All Rights Reserved</copyright>
**/

using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppOSLERLib.BO
{
    [Serializable]
    public class ObjDBInfo
    {
        private DateTime _ctrlData;
        private bool _isPersistent;
        private bool _isLoading;

        private DateTime _criadoEm;
        private DateTime _modificadoEm;
        private Utilizador _criadoPor;
        private Utilizador _modificadoPor;

        public ObjDBInfo()
        {
            this._ctrlData = DateTime.Now;
            IsPersistent = false;
            CriadoPor = null;
            ModificadoPor = null;
            IsLoading = false;
            ObjetoCriadoEm(DateTime.Now);
        }

        public ObjDBInfo(DateTime? criadoEm, Utilizador criadoPor, DateTime? modificadoEm, Utilizador modificadoPor)
        {
            IsLoading = true;
            _ctrlData = DateTime.Now.AddSeconds(-1); // truque...
            IsPersistent = true;
            _criadoPor = criadoPor;
            _modificadoPor = modificadoPor;
            if (criadoEm == null) _criadoEm=DateTime.Now;
            else _criadoEm = (DateTime)criadoEm;
            if (modificadoEm == null) _modificadoEm=DateTime.Now;
            else _modificadoEm = (DateTime)modificadoEm;
        }

        public void ObjetoCriadoEm(DateTime dt)
        {
            _criadoEm = dt;
            ObjetoModificadoEm(dt);
        }
        
        public void ObjetoModificadoEm()
        {
            if (!IsLoading) ObjetoModificadoEm(DateTime.Now);
        }

        public void ObjetoModificadoEm(DateTime dt)
        {
            _modificadoEm = dt;
        }

        public void ObjetoCriadoPor(Utilizador u)
        {
            _criadoPor = u;
            _modificadoPor = u;
        }

        public void ObjetoModificadoPor(Utilizador u)
        {
            _modificadoPor = u;
        }

        public void ObjetoModificadoPor(Utilizador u, DateTime dt)
        {
            ObjetoModificadoEm(dt);
            ObjetoModificadoPor(u);
        }

        private void SetPersistent(bool ispersistent)
        {
            _isPersistent = ispersistent;
        }
        
        /// <summary>
        /// é para verificar se os dados foram modificados
        /// </summary>
        /// <returns></returns>
        public bool IsModified()
        {
            return (_ctrlData < ModificadoEm);
        }
        
        /// <summary>
        /// é para verificar se os dados foram gravados na BD
        /// </summary>
        public bool IsPersistent
        {
            get => _isPersistent;
            set => SetPersistent(value);
        }

        /// <summary>
        /// é para monitorizar o carregamento do objeto
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => _isLoading = value;
        }
        
        /// <summary>
        /// este método deve ser executado imadiatamente após carregar os dados da base de dados
        /// o objetivo é marcar um "ponto" de comparação para detetar se os dados foram modificados
        /// </summary>
        public void DataCheckpointDb()
        {
            if (!IsPersistent) IsPersistent = true;
            _ctrlData = ModificadoEm;
            IsLoading = false;
        }

        [Required]
        public DateTime CriadoEm
        {
            get => _criadoEm;
            set => ObjetoCriadoEm(value);
        }

        [Required]
        public Utilizador CriadoPor
        {
            get => _criadoPor;
            set => ObjetoCriadoPor(value);
        }

        [Required]
        public DateTime ModificadoEm
        {
            get => _modificadoEm;
            set => ObjetoModificadoEm(value);
        }

        [Required]
        public Utilizador ModificadoPor
        {
            get => _modificadoPor;
            set => ObjetoModificadoPor(value);
        }
    }
}